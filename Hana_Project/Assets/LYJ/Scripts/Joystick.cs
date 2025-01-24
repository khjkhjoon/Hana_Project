using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hana.LYJ
{
    public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        public RectTransform background; // Background RectTransform
        public RectTransform handle; // Handle RectTransform

        private Vector2 inputVector; // ���̽�ƽ �Է� ��

        // �巡�� �̺�Ʈ ó��
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background,
                eventData.position,
                eventData.pressEventCamera,
                out pos
            );

            pos.x /= background.sizeDelta.x;
            pos.y /= background.sizeDelta.y;

            inputVector = new Vector2(pos.x * 2, pos.y * 2);
            inputVector = inputVector.magnitude > 1.0f ? inputVector.normalized : inputVector;

            // Handle ��ġ �̵�
            handle.anchoredPosition = new Vector2(
                inputVector.x * (background.sizeDelta.x / 2),
                inputVector.y * (background.sizeDelta.y / 2)
            );
        }

        // �巡�� ���� ��
        public void OnPointerUp(PointerEventData eventData)
        {
            inputVector = Vector2.zero;
            handle.anchoredPosition = Vector2.zero; // Handle ���� ��ġ�� ����
        }

        // �巡�� ���� ��
        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData); // �巡�� ó��
        }

        // ���̽�ƽ �Է� �� ��ȯ
        public Vector2 GetInput()
        {
            return inputVector;
        }
    }
}
