using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRouteData
{
    int _width;
    int _height;
    int[] _values = null;
    int _initiValue = -10;
    int _wallValue = -1;
    int _errorValue = -88;


    //�R���X�g���N�^
    public AutoRouteData(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            Debug.LogError("RouteSearchData�̕��܂��͍�����0�ȉ��ɂȂ��Ă��܂�");
            return;
        }
        _width = width;
        _height = height;
        _values = new int[width * height];

        FillAll(_initiValue); //map�̏�������_initiValue�ōs��
    }

    //�v���p�e�B
    public int Width { get { return _width; } }
    public int Height { get { return _height; } }

    //Getter
    public int GetLength()
    {
        return _values.Length;
    }
    public int GetValue(int x, int y)
    {
        if (IsOutOfRange(x, y))
        {
            Debug.LogError($"�̈�O�̒l���擾���悤�Ƃ��܂���(x,y)=({x},{y})");
            return _errorValue;
        }
        if (IsOnTheEdge(x, y))
        {
            Debug.Log($"IsOnTheEdge({x},{y})��true�ł�");
            return _wallValue;
        }
        return _values[ToSubscript(x, y)];
    }
    public int GetValue(Vector2Int vector)
    {
        return GetValue(vector.x, vector.y);
    }
    public int GetValue(int index)
    {
        if (index < 0 || index > _values.Length)
        {
            Debug.LogError("�̈�O�̒l���K�����悤�Ƃ��܂���");
            return _errorValue;
        }
        return _values[index];
    }

    //Setter
    public void SetValue(int x, int y, int value)
    {
        if (IsOutOfRange(x, y))
        {
            Debug.LogError("�̈�O�ɒl��ݒ肵�悤�Ƃ��܂���");
            return;
        }
        _values[ToSubscript(x, y)] = value;
    }

    public void SetValue(Vector2Int vector, int value)
    {
        SetValue(vector.x, vector.y, value);
    }
    public void SetWall(int x, int y)
    {
        SetValue(x, y, _wallValue);
    }
    //�Y������ϊ�����
    int ToSubscript(int x, int y)
    {
        return x + (y * _width);
    }

    public Vector2Int DivideSubscript(int subscript)
    {
        int xSub = subscript % _width;
        int ySub = (subscript - xSub) / _width; //�����͊���Z
        return new Vector2Int(xSub, ySub);
    }

    bool IsOutOfRange(int x, int y)
    {
        if (x < -1 || x > _width) { return true; }
        if (y < -1 || y > _height) { return true; }

        //map�̒�
        return false;
    }

    bool IsOnTheEdge(int x, int y)
    {
        if (x == -1 || x == _width) { return true; }
        if (y == -1 || y == _height) { return true; }
        return false;
    }

    public void FillAll(int value) //edgeValue�܂ł͏����������Ȃ����Ƃɒ���
    {
        for (int j = 0; j < _height; j++)
        {
            for (int i = 0; i < _width; i++)
            {
                _values[ToSubscript(i, j)] = value;
            }
        }
    }

    public Queue<Vector2Int> SearchShortestRoute(Vector2Int startPos, Vector2Int targetPos)
    {
        Queue<Vector2Int> searchQue = new Queue<Vector2Int>();
        Queue<Vector2Int> resultQue = new Queue<Vector2Int>();
        int i = 1; //1����n�܂邱�Ƃɒ���
        bool isComplete = false;
        int maxDistance = 0;

        SetValue(startPos, 0); //startPos�̕����������͂̔�����s��Ȃ����߁A�����Ōʂɐݒ肷��
        searchQue.Enqueue(startPos);

        while (!isComplete)
        {
            int loopNum = searchQue.Count; //�O�̃��[�v�ŃL���[�ɒǉ����ꂽ���𐔂���
            Debug.LogWarning($"i:{i}�̂Ƃ�loopNum:{loopNum}");
            for (int k = 0; k < loopNum; k++)
            {
                Debug.LogWarning($"PlaceNumAround({searchQue.Peek()})�����s���܂�");
                if (isComplete)
                {
                    break;
                }
                PlaceNumAround(searchQue.Dequeue());
            }
            i++; //�O�̃��[�v�ŃL���[�ɒǉ����ꂽ�������������ꂽ��A�C���f�b�N�X�𑝂₵�Ď��̃��[�v�Ɉڂ�

            if (i > 100) //�������[�v��h���p
            {
                isComplete = true;
                Debug.LogError("SearchShortestRoute��while���Ń��[�v��100��s���Ă��܂��܂���");
            }
        }
        Debug.LogWarning($"StoreRouteAround({targetPos},{maxDistance})�����s���܂�");
        StoreRouteAround(targetPos, maxDistance);

        Debug.LogWarning($"resultQue��{resultQue}");
        return resultQue;

        //�ȉ����[�J���֐�
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
                    if (GetValue(inspectPos) == _initiValue && CanMoveDiagonally(centerPos, inspectPos))
                    {
                        SetValue(inspectPos, i);
                        searchQue.Enqueue(inspectPos);
                        Debug.LogWarning($"({inspectPos})��{i}�ɂ��A�T���p�L���[�ɒǉ����܂����B");
                    }
                    if (inspectPos == targetPos)
                    {
                        isComplete = true;
                        maxDistance = i - 1;
                        Debug.LogWarning($"isComplete��true�ɂ��܂����BmaxDistance{maxDistance}");
                        break; //�T���I��
                    }
                }
            }
        }


        void StoreRouteAround(Vector2Int centerPos, int distance)
        {
            if (distance < 0)
            {
                return; //0�܂�Que�ɓ����Ώ\��
            }

            Debug.LogWarning($"dinstance:{distance}�AmaxDistance:{maxDistance}�̂��߁Adistance == maxDistance��{distance == maxDistance}");
            if (distance == maxDistance) //�ŏI�n�_�̎��肾������8�}�X�ɔ�����s��(9�}�X)
            {
                Vector2Int inspectPos;

                for (int y = -1; y < 2; y++)
                {
                    for (int x = -1; x < 2; x++)
                    {
                        inspectPos = centerPos + new Vector2Int(x, y);
                        if (GetValue(inspectPos) == distance)
                        {
                            resultQue.Enqueue(inspectPos);
                            Debug.Log($"GetValue(inspectPos) ==distance��true�Ȃ��߁AresultQue.Enqueue({inspectPos})�����s���܂��B\ndistance:{distance}");
                            StoreRouteAround(inspectPos, distance - 1);
                        }
                    }
                }
            }
            else
            {
                Debug.Log($"GetValue({centerPos})��{GetValue(centerPos)}�Adistance:{distance}");

                // 5 7 8
                // 3 * 6
                // 1 2 4 �̗D�揇�ʂŔ��肵�Ă���

                Vector2Int[] orderInDirectionArray = new Vector2Int[] { Vector2Int.left + Vector2Int.down, Vector2Int.down, Vector2Int.left, Vector2Int.right + Vector2Int.down, Vector2Int.left + Vector2Int.up, Vector2Int.right, Vector2Int.up, Vector2Int.right + Vector2Int.up };

                foreach (Vector2Int direction in orderInDirectionArray)
                {
                    if (GetValue(centerPos + direction) == distance && CanMoveDiagonally(centerPos, centerPos + direction))
                    {
                        resultQue.Enqueue(centerPos + direction);
                        StoreRouteAround(centerPos + direction, distance - 1);
                        break;
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
    }
}

