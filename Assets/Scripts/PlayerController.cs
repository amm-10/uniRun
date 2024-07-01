using UnityEngine;
using UnityEngine.UI;

// PlayerController는 플레이어 캐릭터로서 Player 게임 오브젝트를 제어한다.
public class PlayerController : MonoBehaviour
{
    public AudioClip deathClip; // 사망시 재생할 오디오 클립
    public float jumpForce = 700f; // 점프 힘

    private int jumpCount = 0; // 누적 점프 횟수
    private bool isGrounded = false; // 바닥에 닿았는지 나타냄
    private bool isDead = false; // 사망 상태

    private Rigidbody2D playerRigidbody; // 사용할 리지드바디 컴포넌트
    private Animator animator; // 사용할 애니메이터 컴포넌트
    private AudioSource playerAudio; // 사용할 오디오 소스 컴포넌트

    public Slider playerSlider;
    public float curHp = 10;
    public float maxHp = 10;

    private void Start()
    {
        // 초기화
        playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
      //  playerSlider = GetComponent<Slider>();
        
    }

    private void Update()
    {
        // 사용자 입력을 감지하고 점프하는 처리
        if (isDead)
        {
            // 사망 시 처리를 더 이상 진행하지 않고 종료
            return;
        }

        CheckHp();

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < 2)
        {
            jumpCount++;

            // 점프 직전에 속도를 순간적으로 제로(0, 0) 로 변경
            playerRigidbody.velocity = Vector2.zero;
            // 리지드바디에 위쪽으로 힘주기
            playerRigidbody.AddForce(new Vector2(0, jumpForce));
            // 오디오 소스 재생
            playerAudio.Play();

            curHp -= 1;
            if(curHp < 0)
            {
                curHp = 10;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space)&&playerRigidbody.velocity.y>0)
        {
            // 점프 키 버튼에서 손을 떼는 순간 && 속도의 y 값이 양수라면(위로 상승중)
            // 현재 속도를 절반으로 변경
            playerRigidbody.velocity = playerRigidbody.velocity * 0.5f;
        }

        // 애니메이터의 Grounded 파라미터를 isGrounded 값으로 갱신
        animator.SetBool("Grounded", isGrounded); 
   }

    private void Die()
    {
        // 애니메이터의 트리거 셋
        animator.SetTrigger("Die");
        // 오디오 소스에 할당된 오디오 클립을 deathClip 으로 변경
        playerAudio.clip = deathClip;
        // 사망 효과음 재생
        playerAudio.Play();

        // 속도를 제로(0, 0)로 변경
        playerRigidbody.velocity = Vector2.zero;
        // 사망 상태를 true 로 변경
        isDead = true;

        GameManager.instance.OnPlayerDead();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 트리거 콜라이더를 가진 장애물과의 충돌을 감지

        // 충돌한 상대방의 태그가 Dead 이고 사망 상태가 아니라면 Die() 실행
        if(other.tag == "Dead" && !isDead)
        {
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 바닥에 닿았음을 감지하는 처리

        // 어떤 콜라이더와 닿았으며, 충돌 표면이 위쪽을 보고있으면
        if (collision.contacts[0].normal.y > 0.7f)
        {
            // isGrounded 를 true 로 변경하고, 누적 점프 횟수를 0으로 리셋
            isGrounded = true;
            jumpCount = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // 바닥에서 벗어났음을 감지하는 처리

        // 어떤 콜라이더에서 떨어진 경우 isGrounded 를 false 로 변경
        isGrounded = false;
    }

    public void CheckHp()
    {
        if(playerSlider!=null) 
        {
            playerSlider.value = curHp / maxHp;
        }
    }
}