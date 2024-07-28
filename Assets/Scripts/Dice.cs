using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.U2D.Animation;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SpriteResolver))]
[RequireComponent(typeof(SpriteLibrary))]
public class Dice : MonoBehaviour {
	private SpriteResolver spriteResolver;
	[SerializeField] private int _value = 6;

	public int Value {
		get => _value;
		set {
			_value = value;
			spriteResolver.SetCategoryAndLabel("Dice", value.ToString());
		}
	}

	private bool isAnimating = false;

	public static Dice Instance;

	private void Awake() {
		if (Instance == null) {
			Instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	private void Start() {
		spriteResolver = GetComponent<SpriteResolver>();
		Value = 6;
	}

	public void RollDiceToNum(int num) {
		var gameState = GameManager.Instance.state;
		if (gameState == GameState.RollingDice && !isAnimating) {
			StartCoroutine(RollDiceToNumCore(num, false));
		}
	}

	public void RollDice() {
		var gameState = GameManager.Instance.state;
		if (gameState == GameState.RollingDice && !isAnimating) {
			int num = Random.Range(1, 7);
			StartCoroutine(RollDiceToNumCore(num, true));
		}
	}

	private IEnumerator RollDiceToNumCore(int num, bool sendEvent) {
		isAnimating = true;
		var prev = Value;
		if (sendEvent) {
			SendDiceRolledEvent(num);
			GameManager.Instance.ChangeState(GameState.SelectingPiece);
		}
		for (int i = 0; i < 4; i++) {
			while (Value == prev) Value = Random.Range(1, 7);
			prev = Value;
			yield return new WaitForSeconds(0.1f);
		}

		Value = num;
		isAnimating = false;
	}

	private void OnMouseUp() {
		if (
			PhotonNetwork.OfflineMode
			|| (ColorsManager.I.currentColor == (GameColor)PhotonNetwork.LocalPlayer.CustomProperties["Color"])
		)
			RollDice();
	}

	private void SendDiceRolledEvent(int num) {
		RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
		PhotonNetwork.RaiseEvent(NetworkEventManager.DiceRolled, num, options, SendOptions.SendReliable);
		var table = new Hashtable { { "DiceValue", num } };
		PhotonNetwork.CurrentRoom.SetCustomProperties(table);
	}
}