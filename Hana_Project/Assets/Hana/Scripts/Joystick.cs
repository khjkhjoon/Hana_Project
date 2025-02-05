using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Hana.LYJ
{
    public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField] private RectTransform joystickHandle; // 조이스틱 핸들 (사용자가 움직이는 부분)
        [SerializeField] private RectTransform joystickBackground; // 조이스틱 배경 (고정된 부분)

        private Vector2 inputVector = Vector2.zero; // 현재 조이스틱 입력 값을 저장하는 변수

        // 수평 입력 값 반환 (3D 이동 적용 가능)
        public float Horizontal => inputVector.x;
        // 수직 입력 값 반환 (3D에서는 Z축 이동으로 활용 가능)
        public float Vertical => inputVector.y;
        // 입력된 방향 벡터 반환 (3D 이동을 위해 Vector3 변환 필요)
        public Vector3 Direction => new Vector3(Horizontal, 0, Vertical);

        // 사용자가 조이스틱을 드래그할 때 실행
        public void OnDrag(PointerEventData eventData)
        {
            // 조이스틱의 중심과 사용자의 터치 위치 차이를 계산
            Vector2 dragPosition = eventData.position - (Vector2)joystickBackground.position;

            // 조이스틱 입력 값을 정규화하여 최대 범위를 제한
            inputVector = (dragPosition.magnitude > joystickBackground.sizeDelta.x / 2f) ?
                dragPosition.normalized : dragPosition / (joystickBackground.sizeDelta.x / 2f);

            // 조이스틱 핸들의 위치를 이동하여 조이스틱이 반응하는 것처럼 보이게 함
            joystickHandle.anchoredPosition = (inputVector * joystickBackground.sizeDelta.x / 2f);
        }

        // 사용자가 조이스틱을 터치했을 때 실행
        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData); // 터치 시에도 드래그 기능이 적용되도록 호출
        }

        // 사용자가 조이스틱에서 손을 뗄 때 실행
        public void OnPointerUp(PointerEventData eventData)
        {
            inputVector = Vector2.zero; // 입력값 초기화
            joystickHandle.anchoredPosition = Vector2.zero; // 조이스틱 핸들 위치 초기화 (중앙으로 복귀)
        }

        public Vector2 GetInput()
        {
            return inputVector; // 현재 입력값 반환
        }
    }
}