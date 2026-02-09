// =============================================================================
// 1. CollisionTriggerObject.cs - 부딪히면 사라지고 트리거 발생하는 오브젝트
// =============================================================================
using UnityEngine;
using UnityEngine.Events;

public class CollisionTriggerObject : MonoBehaviour
{
    [Header("Trigger Settings")]
    public string triggerName = "ItemCollected";
    public bool triggerOnce = true;
    
    [Header("Visual Effects")]
    public bool fadeOut = true;
    public float fadeOutDuration = 0.5f;
    public bool playSound = true;
    public AudioClip triggerSound;
    
    [Header("Particle Effects")]
    public bool createParticles = true;
    public GameObject particlePrefab;
    public ParticleSystem builtInParticles;
    
    [Header("Events")]
    public UnityEvent onTriggerActivated;
    public UnityEvent<string> onTriggerWithName;
    
    [Header("Target Tags")]
    public string[] validTags = { "Player", "Car" };
    
    private bool hasTriggered = false;
    private Renderer objectRenderer;
    private AudioSource audioSource;
    private Collider objectCollider;

    public GameObject firstQuiz;
    
    void Start()
    {
        // 컴포넌트 참조 가져오기
        objectRenderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        objectCollider = GetComponent<Collider>();
        
        // AudioSource가 없으면 자동으로 추가
        if (playSound && audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // 콜라이더가 없으면 경고
        if (objectCollider == null)
        {
            Debug.LogWarning($"{gameObject.name}에 Collider가 없습니다!");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
    }
    
    void HandleCollision(GameObject collidedObject)
    {
        // 이미 트리거된 경우 무시
        if (hasTriggered && triggerOnce)
            return;
        
        // 유효한 태그인지 확인
        if (!IsValidTag(collidedObject.tag))
            return;
        
        // 트리거 실행
        TriggerEffect(collidedObject);
    }
    
    bool IsValidTag(string tag)
    {
        if (validTags.Length == 0) return true; // 태그 제한 없음
        
        foreach (string validTag in validTags)
        {
            if (tag == validTag)
                return true;
        }
        return false;
    }
    
    void TriggerEffect(GameObject triggerObject)
    {
        if (hasTriggered && triggerOnce) return;

        hasTriggered = true;

        firstQuiz.SetActive(true);

        Debug.Log($"{gameObject.name} 트리거 발생! 충돌 대상: {triggerObject.name}");

        // 이벤트 발생
        onTriggerActivated?.Invoke();
        onTriggerWithName?.Invoke(triggerName);

        // 파티클 효과
        if (createParticles)
        {
            CreateParticleEffect();
        }

        // 사운드 재생
        if (playSound && audioSource != null && triggerSound != null)
        {
            audioSource.PlayOneShot(triggerSound);
        }

        // 오브젝트 제거
        if (fadeOut)
        {
            StartCoroutine(FadeOutAndDestroy());
        }
        else
        {
            DestroyObject();
        }
        //if (hasTriggered && triggerOnce) return;

        //hasTriggered = true;

        ////  Canvas가 이미 하나라도 열려있으면 아무것도 하지 않음
        //if (CanvasAlreadyOpen()) return;

        ////  연결된 퀴즈만 활성화
        //if (firstQuiz != null)
        //{
        //    firstQuiz.SetActive(true);
        //}

        //Debug.Log($"{gameObject.name} 트리거 발생! 충돌 대상: {triggerObject.name}");

        //onTriggerActivated?.Invoke();
        //onTriggerWithName?.Invoke(triggerName);

        //if (createParticles) CreateParticleEffect();
        //if (playSound && audioSource != null && triggerSound != null) audioSource.PlayOneShot(triggerSound);

        //if (fadeOut) StartCoroutine(FadeOutAndDestroy());
        //else DestroyObject();

    }

    bool CanvasAlreadyOpen()
    {
        return GameObject.Find("QuizCanvas1")?.activeSelf == true ||
               GameObject.Find("QuizCanvas2")?.activeSelf == true ||
               GameObject.Find("QuizCanvas3")?.activeSelf == true;
    }

    void CreateParticleEffect()
    {
        // 프리팹 파티클 생성
        if (particlePrefab != null)
        {
            GameObject particles = Instantiate(particlePrefab, transform.position, transform.rotation);
            
            // 파티클 시스템이 있으면 자동으로 제거되도록 설정
            ParticleSystem ps = particles.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                float duration = ps.main.duration + ps.main.startLifetime.constantMax;
                Destroy(particles, duration);
            }
            else
            {
                Destroy(particles, 5f); // 기본 5초 후 제거
            }
        }
        
        // 내장 파티클 재생
        if (builtInParticles != null)
        {
            builtInParticles.Play();
        }
    }
    
    System.Collections.IEnumerator FadeOutAndDestroy()
    {
        float elapsedTime = 0f;
        Color originalColor = Color.white;
        
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
        
        // 콜라이더 비활성화 (더 이상 충돌하지 않게)
        if (objectCollider != null)
        {
            objectCollider.enabled = false;
        }
        
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            
            if (objectRenderer != null)
            {
                Color newColor = originalColor;
                newColor.a = alpha;
                objectRenderer.material.color = newColor;
            }
            
            // 크기도 점점 작게
            float scale = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            transform.localScale = Vector3.one * scale;
            
            yield return null;
        }
        
        DestroyObject();
    }
    
    void DestroyObject()
    {
        // 사운드가 재생 중이면 잠시 대기
        if (playSound && audioSource != null && audioSource.isPlaying)
        {
            StartCoroutine(WaitForSoundAndDestroy());
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    System.Collections.IEnumerator WaitForSoundAndDestroy()
    {
        // 렌더러와 콜라이더 비활성화하되 오디오는 유지
        if (objectRenderer != null)
            objectRenderer.enabled = false;
        if (objectCollider != null)
            objectCollider.enabled = false;
        
        yield return new WaitWhile(() => audioSource.isPlaying);
        Destroy(gameObject);
    }
    
    // 수동으로 트리거 발생
    public void ManualTrigger()
    {
        TriggerEffect(gameObject);
    }
    
    // 트리거 리셋 (재사용 가능하게)
    public void ResetTrigger()
    {
        hasTriggered = false;
        
        if (objectRenderer != null)
        {
            objectRenderer.enabled = true;
            Color color = objectRenderer.material.color;
            color.a = 1f;
            objectRenderer.material.color = color;
        }
        
        if (objectCollider != null)
        {
            objectCollider.enabled = true;
        }
        
        transform.localScale = Vector3.one;
    }
}