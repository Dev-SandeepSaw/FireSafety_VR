using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Automation;
using UnityAutomation;
//using UnityEngine.UI;
using TMPro;
public class SubProcess : MonoBehaviour
{
    public Process process;
    [Header("Active This When ..")]
    [Range(0f, 100f)]
    public float percentage;
    [Range(0f, 100f)]
    public float completePercentage;
    public bool isActive;
    [Header("Text and Audio")]
    public string popupText;
    public AudioClip clipAudio;
    public bool completeAfterAudio;
    [Header("Time Complete")]
    [Tooltip("Set to some float time to complete this process after this time, Else set to zero for 'NO' execution")]
    public float completeAfterTime;
    [Space]
    public UnityEvent2[] WhenEnable;
    public UnityEvent2[] WhenComplete;
    public GameObject[] enableDisableObj;
    public Collider[] colliderEnableDisable;
    public GameObject destinationLocation;
    [Space]
    public GameObject[] hightlightObject;
    public Color highlightBlinkColor;
    private List<GameObject> cloneHightlightObject = new List<GameObject>();
    private AudioSource audioSource;
    private Material blinkMaterial;
    private Coroutine startBlinkCoroutine;
    private Coroutine stopBlinkCoroutine;
    private TMP_Text _popupTextManager;
    private DestinationOnEnterTrigger destinationTrigger;
    private void Awake()
    {
        isActive = false;
        blinkMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        blinkMaterial.color = highlightBlinkColor;
        if (process != null)
        {
            process.valueChanged2 += CheckCondition;
            process.isActiveChanged += WhenProcessActive;
        }
        // _processManager = this.transform.parent.parent.gameObject;
        if (_popupTextManager == null || audioSource == null)
        {
            GameObject manager = this.transform.parent.parent.gameObject;
            if ((manager.GetComponent("ProcessManager") as ProcessManager) == null)
            {
                manager = this.transform.parent.parent.parent.gameObject;
            }
            else
                _popupTextManager = manager.GetComponent<ProcessManager>().popupTextMeshPro;
            audioSource = manager.GetComponent<AudioSource>();
        }

    }

    private void Update()
    {
        if (destinationLocation != null)
            _DestinationReached();
    }

    void WhenProcessActive(Process process)
    {
        if (process.IsActive && percentage == 0f)
        {
            WhenEnable.Invoke();
            isActive = true;
            process.activeSubProcessName = gameObject.name;
            _setHierarchyObjColor();
            _EnableDisableObj(true);
            _ColliderEnableDisable(true);
            _CloneSetHightLightMaterial();
            _setPopupText();
            _playAudio();
            _DestinationGetCollider();
            if (completeAfterTime > 0)
            {
                StartCoroutine(_CompleteAfterTime(completeAfterTime));
            }
            Debug.Log(gameObject.name + " Enabled");
        }
    }
    void CheckCondition()
    {
        if (process.IsActive && process.CurrentPercentage == percentage)
        {
            WhenEnable.Invoke();
            isActive = true;
            process.activeSubProcessName = gameObject.name;
            _setHierarchyObjColor();
            _EnableDisableObj(true);
            _ColliderEnableDisable(true);
            _CloneSetHightLightMaterial();
            _DestinationGetCollider();
            _setPopupText();
            _playAudio();
            Debug.Log(gameObject.name + " Enabled");
        }
        if (process.IsActive && process.CurrentPercentage == completePercentage)
        {
            var prettyObj = this.gameObject.GetComponent<PrettyHierarchy.PrettyObject>();
            WhenComplete.Invoke();
            isActive = false;
            _disableHierarchyObjColor();
            _EnableDisableObj(false);
            _ColliderEnableDisable(false);
            _CloneSetHightLightMaterial();

            if (destinationLocation != null)
                destinationLocation.SetActive(false);

            if (startBlinkCoroutine != null)
                StopCoroutine(startBlinkCoroutine);

            if (stopBlinkCoroutine != null)
                StopCoroutine(stopBlinkCoroutine);


            _DestroyCloneHightLightObject();
            Debug.Log(gameObject.name + " Compeleted");
        }



    }

    void _setHierarchyObjColor()
    {
        if (this.GetComponent<PrettyHierarchy.PrettyObject>() == null)
        {
            var prettyObj = this.gameObject.AddComponent<PrettyHierarchy.PrettyObject>();
            prettyObj.backgroundColor = new Color32(150, 233, 255, 255);  //blue color
            prettyObj.textColor = new Color32(0, 0, 0, 255);  //Black Color
        }
    }

    void _disableHierarchyObjColor()
    {
        var prettyObj = this.gameObject.GetComponent<PrettyHierarchy.PrettyObject>();
        Destroy(prettyObj);
    }

    void _EnableDisableObj(bool data)
    {
        if (enableDisableObj != null)
        {
            foreach (GameObject item in enableDisableObj)
            {
                item.SetActive(data);
            }
        }
    }

    void _setPopupText()
    {
        if (popupText != null)
            _popupTextManager.text = popupText;
    }

    void _DestinationGetCollider()
    {
        if (destinationLocation != null)
        {
            if (destinationLocation.GetComponent<Collider>() == null)
            {
                var coll = destinationLocation.AddComponent<BoxCollider>();
                coll.GetComponent<Collider>().isTrigger = true;
                if (destinationLocation.GetComponent("DestinationOnEnterTrigger") as DestinationOnEnterTrigger == null)
                    destinationTrigger = destinationLocation.gameObject.AddComponent<DestinationOnEnterTrigger>();
            }
            else
                Debug.Log("Delete collider from the destination object in subprocess " + this.gameObject.name);
        }
    }

    void _DestinationReached()
    {
        if (destinationTrigger != null)
            if (destinationTrigger.playerEntered)
            {
                destinationTrigger.gameObject.SetActive(false);
                _SubProcessCompeleted();
            }

    }

    void _playAudio()
    {
        if (clipAudio == null)
            return;
        audioSource.clip = clipAudio;
        audioSource.Play();
        if (completeAfterAudio)
            StartCoroutine(_completeAfterAudio(audioSource.clip.length));
    }

    IEnumerator _completeAfterAudio(float audioTime)
    {
        yield return new WaitForSeconds(audioTime);
        _SubProcessCompeleted();


    }

    IEnumerator _CompleteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        _SubProcessCompeleted();
    }

    void _ColliderEnableDisable(bool data)
    {
        if (colliderEnableDisable != null)
        {
            foreach (Collider item in colliderEnableDisable)
            {
                item.enabled = data;
            }
        }
    }

    void _CloneSetHightLightMaterial()
    {
        if (hightlightObject != null)
        {
            foreach (GameObject item in hightlightObject)
            {
                cloneHightlightObject.Add(Instantiate(item));
                foreach (GameObject items in cloneHightlightObject)
                {
                    items.GetComponent<Renderer>().material = blinkMaterial;
                }
            }
            _FixScalePosition();

        }

    }

    void _FixScalePosition()  //fixed by removing parent // then set _pos and _rot to gameobject
    {
        foreach (GameObject item in hightlightObject)  //main object parent removed
        {
            item.transform.parent = null;
            foreach (GameObject items in cloneHightlightObject)   //clone object set transform to main object
            {
                items.transform.position = item.transform.position;
                items.transform.rotation = item.transform.rotation;
                items.transform.localScale = item.transform.localScale;
            }
        }
        if (cloneHightlightObject != null)
            if (startBlinkCoroutine == null)
                startBlinkCoroutine = StartCoroutine(_StartBlink());
    }

    void _DestroyCloneHightLightObject() //on complete destroy
    {
        foreach (GameObject item in cloneHightlightObject)
        {
            Destroy(item);
        }
    }

    IEnumerator _StartBlink()
    {
        yield return new WaitForSeconds(0.7f);  //blinkSpeed //1 
        if (hightlightObject != null)
        {
            foreach (GameObject item in cloneHightlightObject)
            {
                item.GetComponent<MeshRenderer>().enabled = true;
            }
            stopBlinkCoroutine = StartCoroutine(_StopBlink());
            if (startBlinkCoroutine != null)
                StopCoroutine(startBlinkCoroutine);
        }
    }
    IEnumerator _StopBlink()
    {
        yield return new WaitForSeconds(0.7f);  //blinkSpeed //1
        if (hightlightObject != null)
        {
            foreach (GameObject item in cloneHightlightObject)
            {
                item.GetComponent<MeshRenderer>().enabled = false;
            }
            StopCoroutine(stopBlinkCoroutine);
            startBlinkCoroutine = StartCoroutine(_StartBlink());
        }
    }

    [DrawButton]
    public void _SubProcessCompeleted()
    {
        if (process.IsActive)
        {
            StartCoroutine(SetPercentage(completePercentage));
        }
    }
    [DrawButton]
    public void _SubProcessStart()
    {
        if (process.IsActive)
        {
            StartCoroutine(SetPercentage(percentage));
        }
    }

    IEnumerator SetPercentage(float value)
    {
        process.CurrentPercentage = value;
        yield return new WaitForEndOfFrame();
        process.CurrentPercentage = value;
    }
#if UNITY_EDITOR
    [DrawButton]
    private void OnValidate()
    {
        if (!Application.isPlaying && gameObject.activeInHierarchy)
            StartCoroutine(SetProcess());
    }

    IEnumerator SetProcess()
    {
        Transform parent = transform.parent;
        Process getProcess;
        while (true)
        {
            yield return null;
            getProcess = parent.GetComponent<Process>();
            if (getProcess != null)
            {
                process = getProcess;
                break;
            }
            else
            {
                if (parent.parent == null) break;
                parent = parent.parent;
            }
        }

        int childCount = transform.parent.childCount;
        int index = transform.GetSiblingIndex();

        completePercentage = Mathf.Floor((100f / childCount) * (index + 1));
        if (index == 0) percentage = 0f;
        else
            percentage = Mathf.Floor((100f / childCount) * index);
    }
#endif
}
