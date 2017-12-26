using UnityEngine; 
using System.Collections;

public class ExtendedFlycam : MonoBehaviour
{

    /*
	EXTENDED FLYCAM
		Desi Quintans (CowfaceGames.com), 17 August 2012.
		Based on FlyThrough.js by Slin (http://wiki.unity3d.com/index.php/FlyThrough), 17 May 2011.
 
	LICENSE
		Free as in speech, and free as in beer.
 
	FEATURES
		WASD/Arrows:    Movement
		          Q:    Climb
		          E:    Drop
                      Shift:    Move faster
                    Control:    Move slower
                        End:    Toggle cursor locking to screen (you can also press Ctrl+P to toggle play mode on and off).
	*/

    public float cameraSensitivity = 90;
    public float climbSpeed = 4;
    public float normalMoveSpeed = 10;
    public float slowMoveFactor = 0.25f;
    public float fastMoveFactor = 3;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;
    Object[] list;
    int activeDrone = 0;
    bool active;
    Vector3 userOfsets;
    void Start()
    {
        Screen.lockCursor = true;
        list = FindObjectsOfType(typeof(UAV));
        if (list.Length > 0)
            active = true;
        else
            active = false;
        userOfsets = Vector3.zero;
         
    }
    void camraFoucsOn(UAV target)
    {
        // hack need to be genrlaezed for when  drones

        this.transform.position = target.transform.position + (new Vector3(0, 3f, -5f))+userOfsets;
     //   this.transform.localRotation.eulerAngles.Set(30f, 0f, 0f);
    }
    void nextUAV()
    {
        activeDrone++;
        if (activeDrone == list.Length)
            activeDrone = 0;
        refoucs();
    }
    void refoucs()
    {
        userOfsets = Vector3.zero;
        rotationX = 0;
        rotationY = 0;
    }
    void prevUAV()
   {
        activeDrone--;
        if (activeDrone == -1)
            activeDrone = list.Length - 1;
        refoucs();
   }
    public GUIStyle guiStyle;
    string droneName = "";
    private void OnGUI()
    { 
        GUI.Label(new Rect(0, 0, 100, 100), droneName, guiStyle);
    }
    void Update()
    {
        list = FindObjectsOfType(typeof(UAV));
        if(list.Length!=0)
         camraFoucsOn(list[activeDrone] as UAV);
        droneName = list[activeDrone].name;
        rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
        rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, -90, 90);
         
        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up)* Quaternion.Euler(30f, 0, 0);
        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
      
        if (Input.GetKeyDown(KeyCode.N))
            nextUAV();
        if (Input.GetKeyDown(KeyCode.R))
            refoucs();
        if (Input.GetKeyDown(KeyCode.P))
            prevUAV();
        if (Input.GetKeyDown(KeyCode.K))
            Application.Quit();
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            userOfsets += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
            userOfsets += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            userOfsets += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
            userOfsets += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        else
        {
            userOfsets += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
            userOfsets += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
        }


        if (Input.GetKey(KeyCode.Q)) { userOfsets += transform.up * climbSpeed * Time.deltaTime; }
        if (Input.GetKey(KeyCode.E)) { userOfsets -= transform.up * climbSpeed * Time.deltaTime; }

        if (Input.GetKeyDown(KeyCode.End))
        {
            Screen.lockCursor = (Screen.lockCursor == false) ? true : false;
        }
    }
}
 