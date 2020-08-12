using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
	private bool pressed = false;
	public void StartGame() {
		if (pressed) return;
		SceneManager.LoadScene(1);
		pressed = true;
	}

	public void Exit() {
		if (pressed) return;
		Application.Quit();
		pressed = true;
	}
}
