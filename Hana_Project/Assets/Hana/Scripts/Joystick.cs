using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hana.LYJ
{
    public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField] private RectTransform joystickHandle; // ���̽�ƽ �ڵ� (����ڰ� �����̴� �κ�)
        [SerializeField] private RectTransform joystickBackground; // ���̽�ƽ ��� (������ �κ�)

        private Vector2 inputVector = Vector2.zero; // ���� ���̽�ƽ �Է� ���� �����ϴ� ����

        // ���� �Է� �� ��ȯ (3D �̵� ���� ����)
        public float Horizontal => inputVector.x;
        // ���� �Է� �� ��ȯ (3D������ Z�� �̵����� Ȱ�� ����)
        public float Vertical => inputVector.y;
        // �Էµ� ���� ���� ��ȯ (3D �̵��� ���� Vector3 ��ȯ �ʿ�)
        public Vector3 Direction => new Vector3(Horizontal, 0, Vertical);

        // ����ڰ� ���̽�ƽ�� �巡���� �� ����
        public void OnDrag(PointerEventData eventData)
        {
            // ���̽�ƽ�� �߽ɰ� ������� ��ġ ��ġ ���̸� ���
            Vector2 dragPosition = eventData.position - (Vector2)joystickBackground.position;

            // ���̽�ƽ �Է� ���� ����ȭ�Ͽ� �ִ� ������ ����
            inputVector = (dragPosition.magnitude > joystickBackground.sizeDelta.x / 2f) ?
                dragPosition.normalized : dragPosition / (joystickBackground.sizeDelta.x / 2f);

            // ���̽�ƽ �ڵ��� ��ġ�� �̵��Ͽ� ���̽�ƽ�� �����ϴ� ��ó�� ���̰� ��
            joystickHandle.anchoredPosition = (inputVector * joystickBackground.sizeDelta.x / 2f);
        }

        // ����ڰ� ���̽�ƽ�� ��ġ���� �� ����
        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData); // ��ġ �ÿ��� �巡�� ����� ����ǵ��� ȣ��
        }

        // ����ڰ� ���̽�ƽ���� ���� �� �� ����
        public void OnPointerUp(PointerEventData eventData)
        {
            inputVector = Vector2.zero; // �Է°� �ʱ�ȭ
            joystickHandle.anchoredPosition = Vector2.zero; // ���̽�ƽ �ڵ� ��ġ �ʱ�ȭ (�߾����� ����)
        }

        public Vector2 GetInput()
        {
            return inputVector; // ���� �Է°� ��ȯ
        }
    }
}