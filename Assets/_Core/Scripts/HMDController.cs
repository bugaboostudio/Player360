using UnityEngine;
using UnityEngine.SceneManagement;

public class HMDController : MonoBehaviour
{
	public bool isCounting;
	public float countingTime;
	
	private void OnEnable()
    {
        OVRManager.HMDMounted += OVRManager_HMDMounted;
        OVRManager.HMDUnmounted += OVRManager_HMDUnmounted;
    }

	private void Update()
	{
		if (isCounting) countingTime += Time.deltaTime;
	}

	private void OnDisable()
    {
        OVRManager.HMDMounted -= OVRManager_HMDMounted;
        OVRManager.HMDUnmounted -= OVRManager_HMDUnmounted;
    }
	
    private void OVRManager_HMDMounted()
    {
		if (countingTime >= 3)
		{
			SceneManager.LoadScene(0);
		}
		else
		{
			isCounting = false;
			countingTime = 0;
		}
	}

    private void OVRManager_HMDUnmounted()
    {
		isCounting = true;		
    }
}
