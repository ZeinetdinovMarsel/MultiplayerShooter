using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class Fuck : MonoBehaviourPunCallbacks
{
	[Header("Настройки отображения")]
	public Sprite imageToShare;
	public Image uiDisplayImage;
	public float displayDuration = 5f;

	// Этот метод теперь публичный, чтобы его мог вызвать другой скрипт
	public void RequestImageShare()
	{
		if (PhotonNetwork.InRoom && photonView.IsMine)
		{
			photonView.RPC("ShowImageRPC", RpcTarget.Others);
		}
	}

	[PunRPC]
	private void ShowImageRPC()
	{
		StopAllCoroutines();
		StartCoroutine(DisplayRoutine());
	}

	private IEnumerator DisplayRoutine()
	{
		if (uiDisplayImage != null && imageToShare != null)
		{
			uiDisplayImage.sprite = imageToShare;
			uiDisplayImage.gameObject.SetActive(true);
			yield return new WaitForSeconds(displayDuration);
			uiDisplayImage.gameObject.SetActive(false);
		}
	}
}

