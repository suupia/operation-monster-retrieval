using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Function
{



    public static List<Vector2Int> SearchShortestRoute(int width,int height,Vector2Int startPos, Vector2Int endPos)  //startPos����endPos�ւ̍ŒZ�o�H��Ԃ��istartPos�AendPos�ǂ�����܂ށj
    {
        int[] _values = null;  //2�����z��̂悤�Ɉ���
        int _initiValue = -10; //PlaceNumAround�ŏd�����Đ�����u���Ȃ��悤�ɂ��邽�߂ɕK�v
        int _wallValue = -1;   //wall�̃}�X
        int _errorValue = -88;
        List<Vector2Int> shortestRouteList;

        Queue<Vector2Int> searchQue = new Queue<Vector2Int>();
        int n = 1; //1����n�܂邱�Ƃɒ���!!
        bool isComplete = false;
        int maxDistance = 0;


        //�������K�؂��ǂ����`�F�b�N����
        if (width <= 0 || height <= 0)
        {
            Debug.LogWarning("SearchShortestRoute�̕��܂��͍�����0�ȉ��ɂȂ��Ă��܂�");
            return null;
        }
        if (GameManager.instance.mapMGR.GetMapValue(startPos) % GameManager.instance.wallID == 0)
        {
            Debug.LogWarning("SearchShortestRoute��statPos��wallID���܂܂�Ă��܂�");
            return null;
        }
        if (GameManager.instance.mapMGR.GetMapValue(endPos) % GameManager.instance.wallID == 0)
        {
            Debug.LogWarning("SearchShortestRoute��endPos��wallID���܂܂�Ă��܂�");
            return null;
        }


        //������
        _values = new int[width * height];
        shortestRouteList = new List<Vector2Int>();
        FillAll(_initiValue); //map�̏�������_initiValue�ōs��


        //����map���R�s�[���āA�ǂ̃}�X��-1�ɂ���B
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (GameManager.instance.mapMGR.GetMap().GetValue(x, y) % GameManager.instance.wallID == 0)
                {
                    SetValue(x, y, _wallValue);
                }
            }
        }

        //�ǂłȂ��}�X�ɐ��������ԂɐU���Ă���
        Debug.Log($"WaveletSearch�����s���܂� startPos:{startPos}");
        WaveletSearch();


        //�f�o�b�O�p
        string debugCell = "";
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //debugCell += $"({x},{height - y-1}){GetValue(height - y-1, x)}".PadRight(3) + ",";
                debugCell += $"{GetValue(x, height - y - 1)}".PadRight(3) + ",";
            }
            debugCell += "\n";
        }
        Debug.Log($"cell�̒��g��\n{debugCell}");



        //���������ƂɁA�傫���������犪���߂��悤�ɂ��čŒZ���[�g��z��Ɋi�[����
        Debug.Log($"StoreRouteAround({endPos},{maxDistance})�����s���܂�");
        StoreShortestRoute(endPos, maxDistance);

        Debug.Log($"resultRouteList:{string.Join(",", shortestRouteList)}");

        shortestRouteList.Reverse(); //���X�g�𔽓]������
        return shortestRouteList;

        ////////////////////////////////////////////////////////////////////

        //�ȉ����[�J���֐�
       

        void WaveletSearch()
        {
            SetValueByVector(startPos, 0); //startPos�̕����������͂̔�����s��Ȃ����߁A�����Ōʂɐݒ肷��
            searchQue.Enqueue(startPos);

            while (!isComplete)
            {
                int loopNum = searchQue.Count; //�O�̃��[�v�ŃL���[�ɒǉ����ꂽ���𐔂���
                Debug.Log($"i:{n}�̂Ƃ�loopNum:{loopNum}");
                for (int k = 0; k < loopNum; k++)
                {
                    if (isComplete) break;
                    Debug.Log($"PlaceNumAround({searchQue.Peek()})�����s���܂�");
                    PlaceNumAround(searchQue.Dequeue());
                }
                n++; //�O�̃��[�v�ŃL���[�ɒǉ����ꂽ�������������ꂽ��A�C���f�b�N�X�𑝂₵�Ď��̃��[�v�Ɉڂ�

                if (n > 100) //�������[�v��h���p
                {
                    isComplete = true;
                    Debug.Log("SearchShortestRoute��while���Ń��[�v��100��s���Ă��܂��܂���");
                }
            }
        }

        void StoreShortestRoute(Vector2Int centerPos, int distance) //�ċA�I�ɌĂ�
        {

            if (distance < 0) return; //0�܂�Que�ɓ����Ώ\��

            Debug.Log($"GetValue({centerPos})��{GetValueFromVector(centerPos)}�Adistance:{distance}");

            // 5 7 8
            // 3 * 6
            // 1 2 4 �̗D�揇�ʂŔ��肵�Ă���

            Vector2Int[] orderInDirectionArray = new Vector2Int[] { Vector2Int.left + Vector2Int.down, Vector2Int.down, Vector2Int.left, Vector2Int.right + Vector2Int.down, Vector2Int.left + Vector2Int.up, Vector2Int.right, Vector2Int.up, Vector2Int.right + Vector2Int.up };

            foreach (Vector2Int direction in orderInDirectionArray)
            {
                if (GetValueFromVector(centerPos + direction) == distance-1 && CanMoveDiagonally(centerPos, centerPos + direction))
                {
                    shortestRouteList.Add(centerPos);
                    StoreShortestRoute(centerPos + direction, distance - 1);
                    break;
                }
            }
        }


        void PlaceNumAround(Vector2Int centerPos)
        {
            Vector2Int inspectPos;

            //8�}�X���肷��i�^�񒆂̃}�X�̔���͕K�v�Ȃ��j
            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    if (x == 0 && y == 0) continue; //�^�񒆂̃}�X�͔�΂�

                    inspectPos = centerPos + new Vector2Int(x, y);
                    //Debug.Log($"centerPos:{centerPos},inspectPos:{inspectPos}�̂Ƃ��ACanMove(centerPos, inspectPos):{CanMove(centerPos, inspectPos)}");
                    if (GetValueFromVector(inspectPos) == _initiValue && CanMoveDiagonally(centerPos, inspectPos))
                    {
                        SetValueByVector(inspectPos, n);
                        searchQue.Enqueue(inspectPos);
                        Debug.Log($"({inspectPos})��{n}�ɂ��A�T���p�L���[�ɒǉ����܂����B");
                    }
                    else  //�ȉ��f�o�b�O�p
                    {
                        Debug.Log($"{inspectPos}�͏����l�������Ă��Ȃ��@�܂��́@�΂߈ړ��ł����܂���\nGetValueFromVector({inspectPos}):{GetValueFromVector(inspectPos)}, CanMoveDiagonally({centerPos}, {inspectPos}):{CanMoveDiagonally(centerPos, inspectPos)}"); 
                    }
                    //if (inspectPos == targetPos)
                    //{
                    //    isComplete = true;
                    //    maxDistance = i - 1;
                    //    Debug.Log($"isComplete��true�ɂ��܂����BmaxDistance:{maxDistance}");
                    //    break; //�T���I��
                    //}
                    if (inspectPos == endPos && CanMoveDiagonally(centerPos, inspectPos))
                    {
                        isComplete = true;
                        SetValueByVector(inspectPos, n);
                        maxDistance = n;
                        Debug.Log($"isComplete��true�ɂ��܂����BmaxDistance:{maxDistance}");
                        break; //�T���I��
                    }
                }
            }
        }

        bool CanMoveDiagonally(Vector2Int prePos, Vector2Int afterPos)
        {
            Vector2Int directionVector = afterPos - prePos;

            //�΂߈ړ��̎��Ƀu���b�N�̊p���ړ����邱�Ƃ͂ł��Ȃ�
            if (directionVector.x != 0 && directionVector.y != 0)
            {
                //���������̔���
                if (GetValue(prePos.x + directionVector.x, prePos.y) == _wallValue)
                {
                    return false;
                }

                //���������̔���
                if (GetValue(prePos.x, prePos.y + directionVector.y) == _wallValue)
                {
                    return false;
                }
            }

            return true;
        }


        //Getter
        int GetValue(int x, int y)
        {
            if (IsOutOfRange(x, y))
            {
                Debug.LogError($"�̈�O�̒l���擾���悤�Ƃ��܂��� (x,y):({x},{y})");
                return _errorValue;
            }
            if (IsOnTheEdge(x, y))
            {
                Debug.Log($"IsOnTheEdge({x},{y})��true�ł�");
                return _wallValue;
            }
            return _values[ToSubscript(x, y)];
        }
        int GetValueFromVector(Vector2Int vector) //���[�J���֐��̓I�[�o�[���C�h���ł��Ȃ����Ƃɒ���
        {
            return GetValue(vector.x, vector.y);
        }


        //Setter
        void SetValue(int x, int y, int value)
        {
            if (IsOutOfRange(x, y))
            {
                Debug.LogError($"�̈�O�ɒl��ݒ肵�悤�Ƃ��܂��� (x,y):({x},{y})");
                return;
            }
            _values[ToSubscript(x, y)] = value;
        }
        void SetValueByVector(Vector2Int vector, int value)
        {
            SetValue(vector.x, vector.y, value);
        }

        //�Y������ϊ�����
        int ToSubscript(int x, int y)
        {
            return x + (y * width);
        }

        bool IsOutOfRange(int x, int y)
        {
            if (x < -1 || x > width) { return true; }
            if (y < -1 || y > height) { return true; }

            //map�̒�
            return false;
        }
        bool IsOnTheEdge(int x, int y)
        {
            if (x == -1 || x == width) { return true; }
            if (y == -1 || y == height) { return true; }
            return false;
        }

        void FillAll(int value) //edgeValue�܂ł͏����������Ȃ����Ƃɒ���
        {
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    _values[ToSubscript(i, j)] = value;
                }
            }
        }

    }
}

