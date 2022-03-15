using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSizeMGR : MonoBehaviour
{

    [SerializeField] RectTransform rectTransform; //�C���X�y�N�^�[��ŃZ�b�g���Ă���

    bool isActive = true;

    //Setter
    public void SetIsActive(bool flag) //�O������ύX����
    {
        if(flag == false && gameObject.activeInHierarchy) RestoreSize();   //�t���O��false�̎��́A�T�C�Y��߂��Ă���false�ɂ���(�������̎��͂��̃X�N���v�g��t���Ă���I�u�W�F�N�g��false�̂Ƃ�������̂ŁA����͔�΂�)

        isActive = flag;
    }

    [SerializeField] float magnificationPower = 1.15f; //�g��{��
    float time = 0.08f; //�T�C�Y�ύX����������܂ł̎���

    float initiWidth; //�Q�[���J�n���̃T�C�Y��ێ����Ă���
    float initiHeight;

    private void Start()
    {
        initiWidth = rectTransform.sizeDelta.x;
        initiHeight = rectTransform.sizeDelta.y;
    }
    public void IncreaseSize() //EventTrigger��PointerEnter�ŌĂ�
    {
        if (!isActive) return;
        StartCoroutine(MakeButtonBiggerCoroutine());
    }

    public void RestoreSize() //EventTrigger��PointerExit�ŌĂ�
    {
        if (!isActive) return;
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
