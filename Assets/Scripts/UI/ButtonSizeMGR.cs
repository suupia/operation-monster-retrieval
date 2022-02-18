using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSizeMGR : MonoBehaviour
{

    [SerializeField] RectTransform rectTransform; //�C���X�y�N�^�[��ŃZ�b�g���Ă���

    float magnificationPower = 1.15f; //�g��{��
    float time = 0.08f; //�T�C�Y�ύX����������܂ł̎���

    float initiWidth; //�Q�[���J�n���̃T�C�Y��ێ����Ă���
    float initiHeight;

    private void Start()
    {
        initiWidth = rectTransform.sizeDelta.x;
        initiHeight = rectTransform.sizeDelta.y;
    }
    public void MakeButtonBigger() //EventTrigger��PointerEnter�ŌĂ�
    {
        //rectTransform.sizeDelta = new Vector2(initiWidth * magnificationPower, initiHeight * magnificationPower);
        StartCoroutine(MakeButtonBiggerCoroutine());
    }

    public void RestoreTheSize() //EventTrigger��PointerExit�ŌĂ�
    {
        //rectTransform.sizeDelta = new Vector2(initiWidth, initiHeight);
        StartCoroutine(RestoreTheSizeCoroutine());

    }

    IEnumerator MakeButtonBiggerCoroutine()
    {
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            float rate = (magnificationPower - 1) * (t / time) + 1; //���`�⊮
            rectTransform.sizeDelta = new Vector2(initiWidth * rate, initiHeight * rate);
            yield return null;
        }
    }

    IEnumerator RestoreTheSizeCoroutine()
    {
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            float rate = (1 - 1) * (t / time) + 1; //���`�⊮
            rectTransform.sizeDelta = new Vector2(initiWidth * rate, initiHeight * rate);
            yield return null;
        }
    }
}
