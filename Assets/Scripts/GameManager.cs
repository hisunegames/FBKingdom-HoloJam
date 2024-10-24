using System.Collections;
using DYP;
using Febucci.UI;
using UnityEngine;

namespace FBK
{
    public class GameManager : MonoBehaviour
    {

        public static GameManager instance;

        public GameObject player;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public float speed = 1f;
        [Range(0, 4.0f)]
        public float timeScale = 0.5f;
        public float startTimeScale;
        public float startFixedDeltaTime;

        public float miteiruDuration = 3f;
        public float cooldown = 5f;
        [SerializeField]
        bool isMiteiru = false;
        bool onCooldown = false;

        void Start()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }

            if (instance != this)
                Destroy(gameObject);

            startTimeScale = Time.timeScale;
            startFixedDeltaTime = Time.fixedDeltaTime;
        }
        void Update()
        {
            // if (Input.GetKeyDown(KeyCode.E))
            // {
            //     isMiteiru = true;
            // }
        }

        void FixedUpdate()
        {
            if (!onCooldown && isMiteiru)
            {
                StartCoroutine(MiteiruBulletTime());
            }
        }


        IEnumerator MiteiruBulletTime()
        {
            Time.timeScale = timeScale;
            speed = 1 / timeScale;
            Time.fixedDeltaTime = startFixedDeltaTime * timeScale;
            yield return new WaitForSeconds(miteiruDuration * timeScale);
            speed = 1;
            isMiteiru = false;
            onCooldown = true;
            Time.timeScale = startTimeScale;
            Time.fixedDeltaTime = startFixedDeltaTime;
            yield return new WaitForSeconds(cooldown);
            onCooldown = false;

        }
    }
}
