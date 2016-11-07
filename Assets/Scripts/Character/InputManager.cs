using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{

    public float vertical = 0;
    public float horizontal = 0;
    Vector3 cameraDistance;

    public CharacterManager characterManager;
    
    public void InitializeManager()
    {
        characterManager = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterManager>();
        StartCoroutine(GetKeyInput());
    }

    public IEnumerator GetKeyInput()
    {
        while (true)
        {
            yield return null;

            vertical = Input.GetAxisRaw("Vertical");
            horizontal = Input.GetAxisRaw("Horizontal");
            characterManager.Move(vertical, horizontal);

            if (Input.GetKeyDown(KeyCode.T))
            {
                characterManager.UsingPotion();
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                characterManager.NormalAttack();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                characterManager.Jump();
            }

            if (Input.GetButtonDown("Skill1"))
            {
                characterManager.mealstromState = true;
            }

            else if (Input.GetButtonDown("Skill2"))
            {
                characterManager.CutOff();
            }

            else if (Input.GetButtonDown("Skill3"))
            {
                characterManager.Espada();
            }
        }        
    }
}