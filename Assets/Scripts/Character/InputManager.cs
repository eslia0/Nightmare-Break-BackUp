using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{

    public float vertical = 0;
    public float horizontal = 0;
    Vector3 cameraDistance;

    public CharacterManager characterManager;

    void Awake()
    {
        cameraDistance = new Vector3(11f, 6.5f, 0);
    }

    public void InitializeManager()
    {
        characterManager = GameObject.FindWithTag("Player").GetComponent<CharacterManager>();
    }

    void Update()
    {
        GetKeyInput();
        //Camera.main.transform.rotation = new Quaternion (12, -90, 0, Camera.main.transform.rotation.w);
    }

    public void GetKeyInput()
    {
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

    public void LateUpdate()
    {
        //update camera sight
        CameraCtrl();
    }

    public void CameraCtrl()
    {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, characterManager.transform.position + cameraDistance, Time.deltaTime * 10);
    }
}