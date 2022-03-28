using System.Collections;
using UnityEngine;
using System.IO;
using TMPro;

public class MainGame : MonoBehaviour
{
    private Camera cam;
    private Object[] objects1;
    private Object[] objects2;
    private GameObject[] model3d;
    private GameObject actualModel;

    [Header("Parameters of transform")]
    public float horizontalMoveScale = 2.0F;
    public float verticalMoveScale = 2.0F;
    public float scaleParameter = 0.3F;
    public float rotationXScale = 1;
    public float rotationYScale = 1;

    [Header("External Objects")]
    public GameObject modelNameTextObject;

    public GameObject centerOfView;
    public GameObject centerOfWorld;

    private float expScale;
    private int numberOfModel;
    private Vector3 turn;
    private Vector3 move;
    private float actualScale;
    private Transform center;

    private string inputPath;


    void Start()
    {
        cam = Camera.main;
        center = cam.transform.parent;
        cam.transform.position = new Vector3(0, 0, -10);
        transform.SetAsFirstSibling();
        actualScale = 3;
        numberOfModel = 0;
       
        center.position = new Vector3(0, 1, 0);
        CreateOutputDirectory();
        LoadModelsFromInput();
        SpawnModel();

    }

    // Update is called once per frame
    void Update()
    {
        quitOnEscape();
        RotateModel();
        MoveModel();
        ResetCursor();
        ScaleModel();
    }
 private void CreateOutputDirectory(){
        string path = Path.Combine(Application.dataPath, "/../Output");
        Directory.CreateDirectory (path);

        inputPath = Application.dataPath+"/../Input/";
        try{
            if(!Directory.Exists(Application.dataPath+"/../Output")){
                var folder = Directory.CreateDirectory(Application.dataPath+"/../Output"); 
            }
            else{
                Debug.Log("Folder 'Output' istnieje.");
            }

            if(!Directory.Exists(inputPath)){
                var folder = Directory.CreateDirectory(inputPath); 
            }
            else{
                Debug.Log("Folder 'Input' istnieje.");
            }
        }
        catch (IOException ex){
            Debug.Log(ex.Message);
        }
    }
    private void LoadModelsFromInput(){
        objects1 = Resources.LoadAll<GameObject>("Input");
        objects2 = Resources.LoadAll<GameObject>(inputPath);
        this.model3d = new GameObject[objects1.Length + objects2.Length];
        for(int i=0; i < objects1.Length;i++) {
            this.model3d[i] = (GameObject)this.objects1[i];
        }
        for(int i=objects1.Length; i < model3d.Length;i++) {
            this.model3d[i] = (GameObject)this.objects2[i];
        }
        

    }



    private void SpawnModel(){
        actualModel = Instantiate(model3d[numberOfModel], Vector3.zero, Quaternion.identity);
        actualModel.transform.localScale *=20;
        modelNameTextObject.GetComponent<TextMeshProUGUI>().text = "Actual model: " + actualModel.name;
        
    }

    private void RotateModel(){
        if(Input.GetMouseButton(0)){
            turn.x += Input.GetAxis("Mouse Y") * rotationXScale;
            turn.y += Input.GetAxis("Mouse X") * rotationYScale;
        }
        if(Input.GetMouseButton(1)){
            turn.z += Input.GetAxis("Mouse X");

        }
        center.rotation = Quaternion.Euler(-turn.x, turn.y, turn.z);
    }
    private void MoveModel(){
        if(Input.GetMouseButton(2)){
            move.x = horizontalMoveScale * Input.GetAxis("Mouse X");
            move.y = verticalMoveScale * Input.GetAxis("Mouse Y");
            UnityEngine.Cursor.visible = false;
            Vector3 movement = new Vector3(-move.x, -move.y, 0) * expScale * 0.01f;
            center.transform.Translate(movement);
        }
    }
    private void ResetCursor(){
        if(Input.GetMouseButtonUp(2)){
            UnityEngine.Cursor.visible = true;
            var mousePos = Input.mousePosition; 
            mousePos.x -= Screen.width/2;
            mousePos.y -= Screen.height/2;
        }
    }
    private void ScaleModel(){
        actualScale -= Input.mouseScrollDelta.y * scaleParameter;
        expScale = Mathf.Pow(2, actualScale)/2;
        cam.transform.localPosition = new Vector3(0, 0, -expScale - 1);
        centerOfWorld.transform.localScale = new Vector3(1, 1, 1) * (expScale +1)/40;
        centerOfView.transform.localScale = new Vector3(1, 1, 1) * (expScale +1)/20;
    }


    public void TakeScreenshot(){
        GetComponent<Canvas>().enabled = false;
        centerOfWorld.SetActive(false);
        centerOfView.SetActive(false);
        ScreenCapture.CaptureScreenshot(Application.dataPath+"/../Output/Avatar-"+System.DateTime.Now.ToString("yy'-'MM'-'dd'-'hh'-'mm'-'ss")+".png");
        
        StartCoroutine(WaitAfterScreenshot());
    }

    private IEnumerator WaitAfterScreenshot(){
        yield return new WaitForSeconds(1);
        GetComponent<Canvas>().enabled = true;
        centerOfWorld.SetActive(true);
        centerOfView.SetActive(true);
    }

    public void takeNextModel(){
        if(numberOfModel < model3d.Length - 1){
            numberOfModel += 1;
            changeModel();
        }
    }

    public void takePreviousModel(){
        if(numberOfModel > 0){
            numberOfModel -= 1;
            changeModel();
        }
    }

    public void resetTransform(){
        center.position = new Vector3(0, 1, 0);
        turn = Vector3.zero;
        actualScale = 3;
        expScale = Mathf.Pow(2, actualScale)/2;
        center.rotation = Quaternion.Euler(-turn.x, turn.y, turn.z);
        cam.transform.position = new Vector3(0, 0, -5);
        
    }

    private void changeModel(){
            Destroy(actualModel);
            SpawnModel();
    }

    private void quitOnEscape(){
        if (Input.GetKey("escape")) {
            Application.Quit();
        }
    }
}
