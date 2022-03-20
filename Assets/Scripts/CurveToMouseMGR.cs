using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurveToMouseMGR : MonoBehaviour
{
    [SerializeField] GameObject curveToMouseCanvas;
    [SerializeField] GameObject curvePointer;

    [SerializeField] List<GameObject> curvePointers;
    [SerializeField] List<Image> curvePointerImages;
    private int divisionNum;

    private bool isEditingCurvePointers; //CurvePointersを参照している間は、他のところからは参照しないようにする

    [SerializeField] private Color pointerColor;
    private bool isIlluminating;
    private Coroutine illuminatingPointerCroutine; //StopCoroutineの引数にする Pointerを消すときにIlluminatePointersCoroutineをStopする
    private Quaternion pointerAngle; //Curve()内で、与えられたx座標における接線の傾きを代入する

    private void Start()
    {
        curvePointers = new List<GameObject>();
    }
    public void SetCircles(int buttonNum)
    {
        Vector2 startPos = GameManager.instance.selectCharacterButtonMGRs[buttonNum].transform.position;

        Vector2 endPos;
        if(GameManager.instance.mouseEnteredSelectCharacterButtonNum == -1 || GameManager.instance.mouseEnteredSelectCharacterButtonNum == buttonNum)
        {
            endPos = (Vector2)Input.mousePosition;
        }
        else
        {
            endPos = GameManager.instance.selectCharacterButtonMGRs[GameManager.instance.mouseEnteredSelectCharacterButtonNum].transform.position;
        }
        divisionNum = (int)(endPos - startPos).magnitude / 50 + 1;

        if (isEditingCurvePointers) return;
        isEditingCurvePointers = true;

        for (int i = 0; i <= divisionNum; i++)
        {
            if (i < curvePointers.Count)
            {
                curvePointers[i].transform.position = PointerPosInCurve((float)i / (float)divisionNum, startPos, endPos);
                curvePointers[i].transform.rotation = pointerAngle;
            }
            else
            {
                curvePointers.Add(Instantiate(curvePointer, PointerPosInCurve((float)i / (float)divisionNum, startPos, endPos), pointerAngle, curveToMouseCanvas.transform));
                curvePointerImages.Add(curvePointers[curvePointers.Count - 1].GetComponent<Image>());
            }
        }

        while (curvePointers.Count > divisionNum + 1)
        {
            Destroy(curvePointers[curvePointers.Count - 1]);
            curvePointers.RemoveAt(curvePointers.Count - 1);
            curvePointerImages.RemoveAt(curvePointerImages.Count - 1);
        }

        isEditingCurvePointers = false;
    }

    public Vector2 PointerPosInCurve(float r, Vector2 startPos, Vector2 endPos) //平行移動を済ませた座標を返す
    {
        if (r < 0 || r > 1)
        {
            Debug.LogError("Curveの変数は0〜1にしてください r = " + r);
        }
        Vector2 deltaPos = endPos - startPos;

        Vector2 returnPos = Curve(deltaPos.x * r, deltaPos) + startPos;

        return returnPos;
    }

    public Vector2 Curve(float x, Vector2 endPos) //始点を原点としたときの関数 始点(=原点)から終点までのベクトル(=endPos)によって関数を決める
    {
        float a, b; //y=ax^2+bxとする
        float endPosTan = Mathf.Atan(endPos.y / endPos.x);
        float deltaAngle = Mathf.PI / 4 / (Mathf.Abs(endPosTan) * Mathf.Abs(endPosTan) + 1); //原点における接線とendPosベクトルの為す角 接線の傾きが過大にならないように、徐々に小さくしていく

        if (endPos.x < 0)
        {
            deltaAngle *= -1; //常に上に凸にするため
        }

        b = Mathf.Tan(deltaAngle + endPosTan);

        if(Mathf.Abs(b) > 2) //接線の傾きが過大にならないように、徐々に小さくしていく
        {
            deltaAngle *= 1 / (Mathf.Abs(b) - 1); //ラジアン 原点における放物線の接線とendPosベクトルの為す角
        }

        if (deltaAngle > 0 && b < 0) //右に向かう曲線で、接線の傾きが負になるとき
        {
            deltaAngle = 0;
        }
        else if (deltaAngle < 0 && b > 0) //左に向かう曲線で、接線の傾きが正になるとき
        {
            deltaAngle = 0;
        }

        b = Mathf.Tan(deltaAngle + endPosTan);

        a = (endPos.y - endPos.x * b) / (endPos.x * endPos.x);

        float y = a * x * x + b * x;
        //Debug.LogWarning($"a={a}, b={b}, x={x}, endPos={endPos}, y={y}");

        if (endPos.x < 0)
        {
            pointerAngle = Quaternion.Euler(0, 0, Mathf.Atan(2 * a * x + b) * 180 / Mathf.PI + 180); //endPos.x < 0のときの傾きは始点に向かっているので、180度足す
        }
        else
        {
            pointerAngle = Quaternion.Euler(0, 0, Mathf.Atan(2 * a * x + b) * 180 / Mathf.PI);
        }

        return new Vector2(x, y);
    }
    public void DestroyLineBetweenButtonAndPointer()
    {
        if (isEditingCurvePointers) return;
        isEditingCurvePointers = true;

        if(illuminatingPointerCroutine != null)
        {
            StopCoroutine(illuminatingPointerCroutine);
            illuminatingPointerCroutine = null;
        }

        while (curvePointers.Count != 0)
        {
            Destroy(curvePointers[curvePointers.Count - 1]);
            curvePointers.RemoveAt(curvePointers.Count - 1);
            curvePointerImages.RemoveAt(curvePointerImages.Count - 1);
        }

        isEditingCurvePointers = false;
    }

    public void ResetCopyingManualRoue()
    {
        DestroyLineBetweenButtonAndPointer();
        if (GameManager.instance.copyingSelectCharacterButtonNum != -1)
        {
            GameManager.instance.characterManualRouteDatas[GameManager.instance.copyingSelectCharacterButtonNum].DestroyPointerTails();
            GameManager.instance.copyingManualRoute = false;
            GameManager.instance.copyingSelectCharacterButtonNum = -1;
        }
        GameManager.instance.mouseEnteredSelectCharacterButtonNum = -1;
    }

    public void StartIlluminatingPointersCroutine()
    {
        illuminatingPointerCroutine = StartCoroutine(IlluminatePointers());
    }

    private IEnumerator IlluminatePointers()
    {
        while(curvePointerImages.Count != 0)
        {
            StartCoroutine(CallIlluminateImage());
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator CallIlluminateImage()
    {
        for (int i = 0; i < curvePointerImages.Count; i++)
        {
            StartCoroutine(IlluminateImage(curvePointerImages[i]));
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator IlluminateImage(Image image)
    {
        if (image == null) yield break;

        while (CompareColors(image.color, Color.white))
        {
            image.color += Color.white / 20;
            yield return new WaitForSeconds(0.02f);
            if (image == null) yield break;
        }

        while (CompareColors(pointerColor, image.color))
        {
            image.color -= Color.white / 20;
            yield return new WaitForSeconds(0.02f);
            if (image == null) yield break;
        }

        image.color = pointerColor;
    }

    private bool CompareColors(Color c1, Color c2) //c1のrgbが全てc2のrgbより小さいときtrue
    {
        if (c1 == null || c2 == null) return false; //参照が外れているとき

        if (c1.r < c2.r || c1.g < c2.g || c1.b < c2.b) return true;
        else return false;
    }
}
