using System.Collections;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public enum CharacterState
    {
        Idle = 0,
        Run,
        Attack,
        Jump,
        CutOff,
        Maelstrom,
        EsPadaSwordSummon
    }

    Animator animator;
    public Renderer rend;

    public float skillTime;
    public GameObject esPadaSword;
   
    public float charSpeed;
    public float jumpPower;
    public Rigidbody rigdbody;
    public bool mealstromState;

    public bool charVer;
    public bool JumpMove;

    AnimatorStateInfo runState;

    public InputManager inputmanager;

    public GameObject[] enemy;
    public GameObject wall;


    private CharacterStatus stat;
    private int potionCount = 3;
    public int[] skillCoolTime;
    public float esPadaSwordSpeed;
    public float esPadaSwordMatarial;
    public bool esPadaSwordRendTime;
    public GameObject EspadaTemp;
    
    public Animator Animator { get { return animator; } }

    [SerializeField]CharacterState state;
    public CharacterState State { get { return state; } }


    void Start()
    {
        stat = GetComponent<CharacterStatus>();
        animator = GetComponent<Animator>();
        state = CharacterState.Idle;
        enemy = null;
        rigdbody = GetComponent<Rigidbody>();
        mealstromState = false;
        wall = GameObject.FindGameObjectWithTag("Wall");
        JumpMove = false;
       
    }

    void Update()
    {
        enemy = GameObject.FindGameObjectsWithTag("Enemy");

        if (mealstromState)
        {
                Maelstrom();
        }
        if (esPadaSwordRendTime && EspadaTemp != null)
        {
            rend = EspadaTemp.gameObject.GetComponent<Renderer>();
            esPadaSwordMatarial += Time.deltaTime;

            if (EspadaTemp.transform.position.y > 0.1)
            {
                // float esPadaSwordAlpha = 0.8f - esPadaSwordMatarial;
                float esPadaSwordAlpha = 1;
                rend.material.color = new Color(0, 0, 0, esPadaSwordAlpha);
             
                if (esPadaSwordAlpha < 0.5)
                {
                    esPadaSwordMatarial = 0;
                    Destroy(EspadaTemp, 0.5f);
                }

            }
        }

    }

    public void AnimationEnd()
    {
        CharState("Idle");
        JumpMove = false;
        Debug.Log("anima End");
    }

    //char state Method
    public void Move(float ver, float hor)
    {
        if (state == CharacterState.Idle || state == CharacterState.Run)
        {
            runState = this.animator.GetCurrentAnimatorStateInfo(0);

            if (!animator.GetBool("Attack"))
            {
                if (ver != 0 || hor != 0)
                {
                    animator.SetFloat("Ver", ver);
                    animator.SetFloat("Hor", hor);

                    if (ver < 0)
                    {
                        transform.rotation = Quaternion.Euler(new Vector3(0, 180.0f, 0));
                        charVer = false;
                    }
                    else if (ver >= 0)
                    {
                        transform.rotation = Quaternion.Euler(new Vector3(0, 0.0f, 0));
                        charVer = true;
                    }
                    CharState("Run");
                    if (runState.IsName("Run"))
                    {
                        transform.Translate((Vector3.forward * ver - Vector3.right * hor) * Time.deltaTime * charSpeed, Space.World);

                    }
                }
                else if (ver == 0 && hor == 0)
                {
                    animator.SetBool("Run", false);
                    CharState("Idle");
                }
            }
        }
        else if (state == CharacterState.Jump && JumpMove )
        {
            transform.Translate((Vector3.forward * ver - Vector3.right * hor) * Time.deltaTime * charSpeed, Space.World);
        }
        
    }
    public void NormalAttack()
    {
        if (state != CharacterState.CutOff && state != CharacterState.Attack && state != CharacterState.Maelstrom && state != CharacterState.EsPadaSwordSummon)
        {
            Debug.Log(state);
            CharState("Attack");
        }
    }

    public void CheckGrounded()
    {
        if (state == CharacterState.Jump)
        {
            if (transform.position.y <= 0.1f)
            {
                CharState("Idle");
            }
        }
    }

    public void Jump()
    {
        if (state != CharacterState.Jump && state != CharacterState.Maelstrom && state != CharacterState.CutOff && state != CharacterState.Maelstrom)
        {
            CharState("Jump");
        }
    }

    public void JumpForce()
    {
         rigdbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
         JumpMove = true;
    }


    //swordmaster Skill
    public void Espada()
    {
        if (state != CharacterState.Jump && state != CharacterState.Maelstrom && state != CharacterState.CutOff && state != CharacterState.EsPadaSwordSummon)
        {
            CharState("EsPadaSwordSummon");
        }
    }
    public void EspadaSummon()
    {
        float espadaPos;
        if (charVer)
        {
            espadaPos = 10.0f;
            Debug.Log("in");
        }
        else
        {
            espadaPos = -10.0f;
        }

        EspadaTemp = (GameObject)Instantiate(esPadaSword, transform.position + new Vector3(0.0f, 10.0f, espadaPos), Quaternion.Euler(new Vector3(0.0f, -90, 0.0f)));
        EspadaTemp.gameObject.GetComponent<Rigidbody>().AddForce(-Vector3.up * esPadaSwordSpeed, ForceMode.Impulse);
        esPadaSwordRendTime = true;
    }

    public void Maelstrom()
    {
        
        if (state != CharacterState.Run && state != CharacterState.Idle )
        {
            mealstromState = false;
        }
        else
        {
            CharState("Maelstrom");
            float maelstromSpeed = 0.5f;
            float maelstromDistance;

            skillTime += Time.deltaTime;


            for (int i = 0; i < enemy.Length; i++)
            {
                maelstromDistance = Vector3.Distance(this.transform.position, enemy[i].transform.position);

                if (maelstromDistance < 10)
                {
                    enemy[i].transform.Translate((this.transform.position - enemy[i].transform.position) * maelstromSpeed * Time.deltaTime, Space.World);
                }
            }
            if (skillTime >= 1.5f)
            {
                mealstromState = false;
                skillTime = 0;
            }
        }
    }

    public void CutOff()
    {
        if (state != CharacterState.Jump && state != CharacterState.CutOff && state != CharacterState.Maelstrom && state != CharacterState.EsPadaSwordSummon)
        {
            CharState("CutOff");
        }
    }

    public void CutOffMove()
    {
        Instantiate(Resources.Load<GameObject>("Effect/SwordShadow"), new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), Quaternion.identity);


        Ray cutOffDistance = new Ray(this.transform.position, transform.forward); //(this.transform.position);
        RaycastHit rayHit;

        if (Physics.Raycast(cutOffDistance, out rayHit, 5f, 1 << LayerMask.NameToLayer("Map")))
        {
          
            transform.Translate(0, 0, rayHit.distance - 0.5f);
        }
        else
        {
            transform.Translate(0, 0, 5);
            
        }
      
        //animation stop and keyboardinput Lock
    }



    //using Potion
    public void UsingPotion()
    {   //Potion Effect create
        GameObject potionEffect = Instantiate(Resources.Load<GameObject>("Effect/Potion"), transform.position, Quaternion.identity) as GameObject;
        potionEffect.transform.parent = gameObject.transform;
        potionEffect.transform.position += Vector3.up;
        StartCoroutine(Potion());
    }

    IEnumerator Potion()
    {
        for (int i = 0; i < potionCount; i++)
        {
            stat.healthPoint += (int)(stat.healthPoint * 0.3);
            yield return new WaitForSeconds(1f);
        }
    }


    //Animation Method
    void SetStateDefault()
    {
        animator.SetBool("Idle", false);
        animator.SetBool("Run", false);
    }

    public void CharState(string Inputstate)
    {
        SetStateDefault();
        //idle=0,run=1,attack=2
        switch (Inputstate)
        {
            case "Idle":
                state = CharacterState.Idle;
                animator.SetBool("Idle", true);
                break;

            case "Run":
                state = CharacterState.Run;
                animator.SetBool("Run", true);
                break;

            case "Attack":
                state = CharacterState.Attack;
                animator.SetTrigger("Attack");
                break;
            case "Jump":
                state = CharacterState.Jump;
                animator.SetTrigger("Jump");
                break;
            case "CutOff":
                state = CharacterState.CutOff;
                animator.SetTrigger("CutOff");
                break;
            case "Maelstrom":
                state = CharacterState.Maelstrom;
                animator.SetTrigger("Maelstrom");
                break;
            case "EsPadaSwordSummon":
                state = CharacterState.EsPadaSwordSummon;
                animator.SetTrigger("EsPadaSwordSummon");
                break;
        }
    }


    //NetWork
    public CharacterState GetCharacterState(int state)
    {
        switch (state)
        {
            case 0:
                return CharacterState.Idle;

            case 1:
                return CharacterState.Run;

            case 2:
                return CharacterState.Attack;

            default:
                return CharacterState.Idle;
        }
    }

    public void SetState(CharacterStateData newStateData)
    {
        Debug.Log("상태 설정");
        animator.SetBool("Direction", newStateData.direction);
        animator.SetFloat("Ver", newStateData.ver);
        animator.SetFloat("Hor", newStateData.hor);
        transform.position = new Vector3(newStateData.posX, newStateData.posY, newStateData.posZ);

        //animator.SetBool(GetCharacterState(newStateData.state).ToString(), true);
    }
}

