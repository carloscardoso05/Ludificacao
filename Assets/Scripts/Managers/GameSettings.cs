using System;
using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Managers {
	/// <summary>
	///     Gerencia as configurações/opções da partida;
	/// </summary>
	public class GameSettings : MonoBehaviourPunCallbacks {
		/// <summary>
		///     Instância para garantir que só haverá uma desta classe.
		/// </summary>
		public static GameSettings Instance;

		private void Awake() {
			if (Instance == null)
				Instance = this;
			else
				Destroy(gameObject);
			DontDestroyOnLoad(gameObject);
		}

		/// <summary>
		///     Retorna o bônus da dificuldade especificada.
		/// </summary>
		/// <param name="difficulty">Dificuldade (0, 1 ou 2, respectivamente fácil, média ou difícil).</param>
		/// <returns>O bônus da dificuldade.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Caso a dificuldade não esteja entre 0 e 2.</exception>
		public int GetDifficultyBonus(int difficulty) {
			return difficulty switch {
				0 => settings.EasyBonus,
				1 => settings.MediumBonus,
				2 => settings.HardBonus,
				_ => throw new ArgumentOutOfRangeException(
					$"Valores válidos para a dificuldade são 0, 1 e 2. Recebeu {difficulty}")
			};
		}

		/// <summary>
		///     Retorna o temporizador da dificuldade especificada.
		/// </summary>
		/// <param name="difficulty">Dificuldade (0, 1 ou 2, respectivamente fácil, média ou difícil).</param>
		/// <returns>O temporizador da dificuldade.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Caso a dificuldade não esteja entre 0 e 2.</exception>
		public float GetDifficultyTimer(int difficulty) {
			return difficulty switch {
				0 => settings.EasyTimer,
				1 => settings.MediumTimer,
				2 => settings.HardTimer,
				_ => throw new ArgumentOutOfRangeException(
					$"Valores válidos para a dificuldade são 0, 1 e 2. Recebeu {difficulty}")
			};
		}

		/// <summary>
		///     Retorna uma função que define o temporizador da dificuldade.
		/// </summary>
		/// <param name="difficulty">Dificuldade (0, 1 ou 2, respectivamente fácil, média ou difícil).</param>
		/// <returns>A função que define o temporizador da dificuldade.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Caso a dificuldade não esteja entre 0 e 2.</exception>
		public Action<string> SetDifficultyTimer(int difficulty) {
			return newTimer => {
				switch (difficulty) {
					case 0:
						settings.EasyTimer = float.Parse(newTimer);
						break;
					case 1:
						settings.MediumTimer = float.Parse(newTimer);
						break;
					case 2:
						settings.HardTimer = float.Parse(newTimer);
						break;
					default:
						throw new ArgumentOutOfRangeException(
							$"Valores válidos para a dificuldade são 0, 1 e 2. Recebeu {difficulty}");
				}
			};
		}

		/// <summary>
		///     Retorna uma função que define o bônus da dificuldade.
		/// </summary>
		/// <param name="difficulty">Dificuldade (0, 1 ou 2, respectivamente fácil, média ou difícil).</param>
		/// <returns>A função que define o bônus da dificuldade.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Caso a dificuldade não esteja entre 0 e 2.</exception>
		public Action<string> SetDifficultyBonus(int difficulty) {
			return newBonus => {
				switch (difficulty) {
					case 0:
						settings.EasyBonus = int.Parse(newBonus);
						break;
					case 1:
						settings.MediumBonus = int.Parse(newBonus);
						break;
					case 2:
						settings.HardBonus = int.Parse(newBonus);
						break;
					default:
						throw new ArgumentOutOfRangeException(
							$"Valores válidos para a dificuldade são 0, 1 e 2. Recebeu {difficulty}");
				}
			};
		}

		/// <summary>
		///     Envia o evento de que uma configuração ou opção foi definida.
		/// </summary>
		/// <param name="settings">Nova configuração.</param>
		private static void SendSettingsDefinedEvent(Settings settings) {
			RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
			PhotonNetwork.RaiseEvent(NetworkEventManager.SettingsDefined, JsonConvert.SerializeObject(settings),
				options, SendOptions.SendReliable);
		}


		#region Bonus

		/// <summary>
		///     Configurações da partida.
		/// </summary>
		public Settings settings = new();

		public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
			base.OnPlayerEnteredRoom(newPlayer);
			SendSettingsDefinedEvent(settings);
		}

		private void Start() {
			settings.SettingsChanged += SendSettingsDefinedEvent;
		}

		#endregion
	}
}