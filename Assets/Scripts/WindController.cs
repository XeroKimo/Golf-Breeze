using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.UI;

public class WindController : MonoBehaviour
{
    public Rigidbody playerBody;
    public float minWindPower;
    public float maxWindPower;
    public float minWindDuration;
    public float maxWindDuration;
    const float windDurationIncrements = 0.5f;
    public float windPower;
    public float windDuration;
    public float noInput = 0.1f;
    public Vector3 windDirection;

    public Image powerElement;

    private float fireWindDuration;
    private Vector3 initialPosition;
    private float accumulatedTime = 0;

    private bool canFire = true;
    public Vector3 lastHitPosition;

    public Action onStrokeTaken;
    public Action onWindStop;
    public Action onBallStop;
    public Action<float, float> onWindDurationChanged;
    public ParticleSystem windParticle;

    public bool ballMoving => !(playerBody.velocity.magnitude < 0.05f && playerBody.velocity.y <= 0.01f);

    public AudioSource audioSource;
    public AudioClip windSFX;
    // Start is called before the first frame update
    void Start()
    {
        windParticle.Stop();
    }

    private void OnDisable()
    {
        playerBody.velocity = Vector3.zero;
        playerBody.angularVelocity = Vector3.zero;
        accumulatedTime = 0;
        windPower = 0;
        if(powerElement)
            powerElement.gameObject.SetActive(false);
    }

    private void Update()
    {
        float oldWindDuration = windDuration;
        windDuration += Input.mouseScrollDelta.y * windDurationIncrements;
        windDuration = Mathf.Clamp(windDuration, minWindDuration, maxWindDuration);
        if(oldWindDuration != windDuration)
        {
            onWindDurationChanged?.Invoke(oldWindDuration, windDuration);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetLastPosition();
        }
        if (canFire)
        {
            if (Input.GetMouseButtonDown(0))
            {
                initialPosition = Input.mousePosition;
                Cursor.visible = false;
                powerElement.gameObject.SetActive(true);
                powerElement.transform.position = Input.mousePosition;
            }
            if(Input.GetMouseButton(0) && Input.GetMouseButtonDown(1))
            {
                powerElement.gameObject.SetActive(false);
                Cursor.visible = true;
            }
            if (powerElement.gameObject.activeSelf)
            {
                windDirection = Input.mousePosition - initialPosition;
                windDirection.z = windDirection.y;
                windDirection.y = 0;
                windDirection.Normalize();
                Vector2 offset = (Input.mousePosition - initialPosition);

                powerElement.rectTransform.sizeDelta = new Vector2(Mathf.Min(offset.magnitude, (Screen.height / 2)), powerElement.rectTransform.sizeDelta.y);
                powerElement.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(windDirection.z, windDirection.x) * Mathf.Rad2Deg);
            }
            if (Input.GetMouseButtonUp(0) && powerElement.gameObject.activeSelf)
            {
                powerElement.gameObject.SetActive(false);
                Cursor.visible = true;
                windDirection = Input.mousePosition - initialPosition;
                windDirection.z = windDirection.y;
                windDirection.y = 0;
                windParticle.transform.rotation = Quaternion.Euler(0, 90 - Mathf.Atan2(windDirection.z, windDirection.x) * Mathf.Rad2Deg, 0);
                windParticle.Play();
                float normalizedPower = Mathf.Clamp(windDirection.magnitude / (Screen.height / 2), 0, 1);
                onStrokeTaken?.Invoke();
                audioSource.clip = windSFX;
                audioSource.Play();
                audioSource.volume = 1;

                lastHitPosition = playerBody.transform.position;
                fireWindDuration = windDuration;
                windPower = Mathf.Lerp(minWindPower, maxWindPower, normalizedPower);
                windDirection.Normalize();
            }
        }
    }

    public void ResetLastPosition()
    {
        playerBody.velocity = Vector3.zero;
        playerBody.angularVelocity = Vector3.zero;
        playerBody.transform.position = lastHitPosition;
        accumulatedTime = 0;
        windPower = 0;
        powerElement.gameObject.SetActive(false);
        audioSource.Stop();
        audioSource.time = 0;
        windParticle.Stop();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (windPower > 0)
        {
            playerBody.AddForce(windDirection * windPower, ForceMode.Acceleration);
            accumulatedTime += Time.fixedDeltaTime;
            float percentAccumulated = accumulatedTime / fireWindDuration;
            const float falloffThreshold = 0.8f;
            if (percentAccumulated >= falloffThreshold)
            {
                audioSource.volume = Mathf.Lerp(1, 0, (percentAccumulated - falloffThreshold) / (1 - falloffThreshold));
            }
            if (accumulatedTime >= fireWindDuration)
            {
                windParticle.Stop();
                audioSource.Stop();
                audioSource.time = 0;
                windPower = 0;
                accumulatedTime = 0;
                onWindStop?.Invoke();
            }
        }
        bool previousCanFire = canFire;
        canFire = !ballMoving;
        if(previousCanFire != canFire && canFire)
        {
            onBallStop?.Invoke();
        }
    }
}
