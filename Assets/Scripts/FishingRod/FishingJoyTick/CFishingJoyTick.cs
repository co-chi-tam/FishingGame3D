using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;

namespace UICustom.FishingGame3D {
	public class UIFishingJoytick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
	{

		[SerializeField]	private Image m_BackgroundImage;
		[SerializeField]	private Image m_KnobImage;
		[SerializeField]	private bool m_AlwayShow;

		public Vector3 InputDirectionXZ { get; set; }
		public Vector3 InputDirectionXY { get; set; }

		private RectTransform m_RectTransform;
		private bool m_EnableJoytick;

		protected virtual void Awake() {
			this.m_RectTransform = this.transform as RectTransform;
			this.m_EnableJoytick = false;
		}

		protected virtual void Start() {
			this.InputDirectionXZ = Vector3.zero;
			this.InputDirectionXY = Vector3.zero;
			this.SetEnableJoytick (this.m_AlwayShow);
		}

		public virtual void SetEnable(bool value) {
			this.gameObject.SetActive (value);
			this.SetEnableJoytick (value);
		}

		public virtual bool GetEnableJoytick() {
			return m_EnableJoytick;
		}

		public virtual void SetEnableJoytick(bool value) {
			this.m_BackgroundImage.gameObject.SetActive (value);
			this.m_KnobImage.gameObject.SetActive (value);
			this.m_EnableJoytick = !value;
		}

		protected virtual void Reset() {
			this.InputDirectionXZ = Vector3.zero;
			this.InputDirectionXY = Vector3.zero;
			this.m_BackgroundImage.rectTransform.anchoredPosition = Vector2.zero;
			this.m_KnobImage.rectTransform.anchoredPosition = Vector2.zero;
		}

		#region Interface implementation

		public void OnBeginDrag (PointerEventData eventData)
		{

		}

		public void OnDrag (PointerEventData eventData)
		{
			var pos = Vector2.zero;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(m_BackgroundImage.rectTransform, 
				eventData.position, 
				eventData.pressEventCamera, 
				out pos)) 
			{
				pos.x = (pos.x / m_BackgroundImage.rectTransform.sizeDelta.x);	
				pos.y = (pos.y / m_BackgroundImage.rectTransform.sizeDelta.y);	

				InputDirectionXZ = new Vector3 (pos.x * 2f, 0f, pos.y * 2f);
				InputDirectionXY = new Vector3 (pos.x * 2f, pos.y * 2f, 0f);
				InputDirectionXZ = InputDirectionXZ.magnitude > 1f ? InputDirectionXZ.normalized : InputDirectionXZ;
				InputDirectionXY = InputDirectionXY.magnitude > 1f ? InputDirectionXY.normalized : InputDirectionXY;
				m_KnobImage.rectTransform.anchoredPosition = new Vector2 (InputDirectionXZ.x * (m_BackgroundImage.rectTransform.sizeDelta.x / 3f) , 
					InputDirectionXZ.z * (m_BackgroundImage.rectTransform.sizeDelta.y / 3f));
			}
		}

		public void OnEndDrag (PointerEventData eventData)
		{
			this.Reset ();
		}

		public void OnPointerDown (PointerEventData eventData)
		{
			if (this.m_AlwayShow == false) {
				this.SetEnableJoytick (true);
			}
			this.m_BackgroundImage.rectTransform.anchoredPosition = eventData.position - this.m_RectTransform.anchoredPosition;
			this.OnDrag (eventData);
		}

		public void OnPointerUp (PointerEventData eventData)
		{
			if (this.m_AlwayShow == false) {
				this.SetEnableJoytick (false);
			}
			this.Reset ();
		}

		#endregion


	}
}

