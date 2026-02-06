using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using StarterAssets;
public class CameraController : MonoBehaviour
{
    [Header("Gameobject Assign")]
    public CinemachineVirtualCamera virtualCamera;
    private Rigidbody collapseCamRigidbody;
    public Transform playerCameraRoot;
    public Animator animator;
    public FirstPersonController firstPersonController;
    private GameObject hourglass;

    [Header("Camera Fall")]
    public float fallForce = 10.0f;
    public float forwardTransformSpeed = 2.0f;
    public float forwardThrustForce = 5.0f; // Force to simulate forward thrust
    public float rotationSpeed = 2.0f;
    public float stopDelay = 2.0f;
    public float dragIncreaseRate = 5.0f;
    public float forwardTiltAngle = 45.0f; // Initial forward tilt angle
    private bool isFalling = false;
    private Vector3 cameraVectorPosition;

    public Transform playerCameraCollapsePosition;

    private void Start()
    {
        collapseCamRigidbody = GetComponent<Rigidbody>();
        collapseCamRigidbody.isKinematic = true;
        Debug.Log("virtual Camera" + virtualCamera.name);
        hourglass = GameObject.Find("HourGlass");
        //virtualCamera = GetComponent<CinemachineVirtualCamera>();

    }
    private void Update()
    {
        if (isFalling)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.0f))
            {
                StartCoroutine(GraduallyStopCamera());
                isFalling = false;
            }
        }
    }


    public void OnPlayerDeath()
    {

        if (virtualCamera != null)
        {
            firstPersonController.SetIfPlayerCanMove(false);
            virtualCamera.Priority = 11;
            CameraFall();
        }

    }

    private void CameraFall()
    {
        //set the falling cam location to where the main camera is
        transform.position = playerCameraRoot.position;
        transform.rotation = playerCameraRoot.rotation;
        isFalling = true;

        //CameraCollapse();
        StartCoroutine(CameraTransform());

    }

    private IEnumerator CameraTransform()
    {

        float targetZPosition = playerCameraCollapsePosition.position.z;
        float targetYPosition = playerCameraCollapsePosition.position.y + 1f;
        float targetXPosition = playerCameraCollapsePosition.position.x;

        Quaternion originalRotation = transform.localRotation;

        while (Mathf.Abs(targetYPosition - transform.position.y) >= Mathf.Epsilon)
        {
            cameraVectorPosition.y = Mathf.Lerp(transform.localPosition.y, targetYPosition, forwardTransformSpeed / 2);

            transform.localPosition = cameraVectorPosition;
        }


        Quaternion targetRotation = Quaternion.Euler(60f, 0f, 50f); // Adjust as needed for the correct down-facing rotation


        while (Mathf.Abs(targetZPosition - transform.position.z) >= Mathf.Epsilon ||
               Mathf.Abs(targetXPosition - transform.position.x) >= Mathf.Epsilon)
        {
            // Interpolate position
            cameraVectorPosition.z = Mathf.Lerp(transform.localPosition.z, targetZPosition, forwardTransformSpeed * Time.deltaTime * 2);
            cameraVectorPosition.x = Mathf.Lerp(transform.localPosition.x, targetXPosition, forwardTransformSpeed * Time.deltaTime * 2);
            transform.localPosition = cameraVectorPosition;

            // Interpolate rotation towards the ground
            transform.localRotation = targetRotation;

            // Apply random flinch rotation
            float[] flinchIntensity = { -0.05f, 0.05f };
            int temp = Random.Range(0, 2);
            float flinchAngleX = flinchIntensity[temp]; // Adjust the range for desired flinch intensity

            temp = Random.Range(0, 2);
            float flinchAngleY = flinchIntensity[temp];

            temp = Random.Range(0, 2);
            float[] flinchIntensityZ = { -0.05f, 0.05f };
            float flinchAngleZ = flinchIntensityZ[temp];

            Quaternion flinchRotation = Quaternion.Euler(flinchAngleX, flinchAngleY, flinchAngleZ);
            transform.localRotation *= flinchRotation;

            yield return null;
        }
    }
    private void CameraCollapse()
    {
        collapseCamRigidbody.isKinematic = false;
        collapseCamRigidbody.useGravity = true;
        //collapseCamRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        collapseCamRigidbody.AddForce(Vector3.down * fallForce, ForceMode.Impulse);
        collapseCamRigidbody.AddForce(transform.forward * forwardThrustForce, ForceMode.Impulse);
    }

    private IEnumerator GraduallyStopCamera()
    {
        yield return new WaitForSeconds(stopDelay);
        float maximumTime = 5f;
        float elapsedTime = 0f;

        while (collapseCamRigidbody.velocity.magnitude > 0.1f || collapseCamRigidbody.angularVelocity.magnitude > 0.1f)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > maximumTime)
            {
                Debug.LogWarning("Camera stop took too long, resetting to zero velocity and angular velocity.");
                StartCoroutine(ResetCollapseCamera());
                break; // Stop after a maximum time to prevent infinite loop
            }
            collapseCamRigidbody.drag += dragIncreaseRate * Time.deltaTime;
            collapseCamRigidbody.angularDrag += dragIncreaseRate * Time.deltaTime;
            yield return null;
        }

        collapseCamRigidbody.velocity = Vector3.zero;
        collapseCamRigidbody.angularVelocity = Vector3.zero;

        EyeBlinkingEffect();
        StartCoroutine(ResetCollapseCamera());
    }
    private void EyeBlinkingEffect()
    {
        animator.SetTrigger("isBlinking");
    }

    private IEnumerator ResetCollapseCamera()
    {
        yield return new WaitForSeconds(3.5f);
        virtualCamera.Priority = 9;
        GameObject.Find("GameOverCanvas").GetComponent<Canvas>().enabled = true;

        Vector3 finalPosition = new Vector3(0, 0, 2);
        Vector3 initialPosition = hourglass.transform.position;
        Quaternion finalRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        Quaternion initialRotation = hourglass.transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < 3)
        {
            float t = elapsedTime / 3;
            t = Mathf.Pow(t, 0.6f);

            hourglass.transform.position = Vector3.Lerp(initialPosition, finalPosition, t);
            hourglass.transform.rotation = Quaternion.Lerp(initialRotation, finalRotation, t);


            elapsedTime += Time.deltaTime;
            yield return null;
        }

        hourglass.transform.position = finalPosition;
        hourglass.transform.rotation = finalRotation;
        hourglass.transform.Find("hourglass/Sand").gameObject.SetActive(true);

        yield return new WaitForSeconds(4f);

        animator.ResetTrigger("isBlinking");

        DataManager.Instance._thirstyOnce1 = true;
        DataManager.Instance._thirstyOnce2 = true;
        DataManager.Instance._thirstyOnce3 = true;
        DataManager.Instance._reviveInfo = true;

        DataManager.Instance.setPlayerThirst(100f);
        DataManager.Instance._getGemstone = false;
        DataManager.Instance.SetStartPoint(new Vector3(-20.3f, -1.5f, 28));
        firstPersonController.SetIfPlayerCanMove(true);
        SceneManager.LoadScene("Main");//Change it to Main Scene later
    }
}