using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour//this class handles the game logic
{
    [SerializeField] private CameraController camControl; // handles camera motion
    [SerializeField] private GratedCircle gratedCircle;
    [SerializeField] private GameObject targetRingParent;//parent object of target ring. setting this inactive disables the target ring as a whole
    [SerializeField] private GameObject targetRing; //this is the actual target ring and gets disabled/enabled on trial start/end
    private IODevice io; // holds the type of IO device that we will use
    [SerializeField] private IODevice keyboard;// the keyboard is used for dev mode (sending keyboard commands to game)
    [SerializeField] private ArduinoIO arduinoIO; // the arduinoIO is used for hardware control (mouse controls game)

    [SerializeField] private GameObject coverPanel; //UI panel that covers the screen when the natural background is off
    [SerializeField] private GameObject failPanel; // black UI panel that covers the screen on fail
    [SerializeField] private GameObject environment; // parent object of all 3d environment objects

    [SerializeField] private Text devModeText; // title text of the dev mode screen
   
    [SerializeField] private CanvasManager cm; // deals with switching canvases (UI screens)

    [SerializeField] private InputFields welcomeFields; // set of input fields with the initial prompt
    [SerializeField] private Text welcomeErrorText; // on screen text used for errors in the welcome screen

    [SerializeField] private SoundMaker sound;

    [SerializeField] private GameObject continueButton;// the button that returns from the game in the pause menu
    [SerializeField] private Text pauseMenuText;

    private bool selfEnabled = true;// used to turn off Update(). selfEnabled allows us to keep enabled on for coroutines

    private int numTrials;//how many trials do we want the mouse to be able to play

    private const float controlPauseTime = 1f;// how long in seconds to pause before the mouse is able to control the game.
    //we want a pause so that the mouse sees the stimulus and makes a conscious decision before turning the joystick

    private const float successPauseTime = 2f;//how long to pause after the mouse succeeeds
    private const float failPauseTime = 4f;//how long to pause after the mouse fails

    private const float timeOutTime = 10f;//how much time does the mouse have to make a choice

    private bool waitingForIR;//state machine variable (two states: waiting for IR sensor, or playing the game)

    private float startTime;//time that the game started. Used to keep track of how long it took to complete all trials

    private void Awake()
    {
        failPanel.SetActive(false);// fail panel should be inactive by default
        WelcomeError("");//clear welcome menu error text
        cm.Welcome();//swtich to the welcome menu
        SetState(false);//set game to not accept input
        selfEnabled = false;//dont run the game until the user makes choices at the main menu
    }
    public void TryToStartGame()
    {
        try
        {
            welcomeFields.Save();//save user inputs for next time, and put inputs into the static SessionData class

            //how many trials do we want to run?
            numTrials = int.Parse(SessionData.numTrials);
            if (numTrials < 1) throw new System.Exception("Invalid number of trials.");//make sure we run at least 1 trial

            Results.Malloc(numTrials);// allocate memory for info about all the trials
            Results.CreateSaveFile(SessionData.saveDirectory,SessionData.mouseID,int.Parse(SessionData.sessionNumber));

            //will we use the naturalistic background or not?
            bool natBackground = int.Parse(SessionData.naturalisticBackground) > 0;
            targetRingParent.SetActive(!natBackground);//use the green circle if no natural background
            coverPanel.SetActive(!natBackground);//covers screen if no nat bg
            environment.SetActive(natBackground);//enable nat bg if applicable
            gratedCircle.SetScalingMode(natBackground);// if we are using the nat bg we want the grated circle to scale as if it is part of the environment

            if (SessionData.mouseID.Equals("dev",System.StringComparison.OrdinalIgnoreCase))// allow user to enter devMode (keyboard inputs) by setting the mouseID todev
            {
                io = keyboard;// devmode uses keyboard as io device
                camControl.SetIODevice(io);
                devModeText.text = "You have entered developer mode by setting the mouse ID to " + SessionData.mouseID + ". Do you want to continue?";
                cm.DevMode();//warns user about dev mode
                return;
            }
            else
            {
                //otherwise we will use the hardware connected to the arduino
                io = arduinoIO as IODevice;
                camControl.SetIODevice(io);
                arduinoIO.Connect(SessionData.port);//connect to user specified port
            }


        }
        catch (System.Exception e)
        {
            //display errors on screen
            WelcomeError(e);
            return;
        }
        StartGame();
        
    }
    public void StartGame()
    {
        startTime = Time.time;// keep track of when game was started
        io.CloseServos();// servos should start closed. This also calibrates the encoder
        cm.HUD();// switch to the HUD canvas. This contains the grated circle
       
        WaitForIR();
        
    }
    private void Update()
    {
        if (!selfEnabled) return;// selfEnabled is used like enabled but allows us to keep enabled on for coroutines

        if (waitingForIR)
        {
            // start the trial if the IR sensor is tripped
            if (io.ReadIR())
            {
                waitingForIR = false;
                StartTrial();
            }
            return;
        }


        //here we check for mouse success/failure. If the grated circle is not in the active, ignore its position
        if (!gratedCircle.gameObject.activeSelf) return;
        if(gratedCircle.AtCenter())
        {
            Success();
        }
        if (gratedCircle.OutOfBounds())
        {
            Hit();
        }
    }
    private void WaitForIR()
    {
        waitingForIR = true;
        SetState(false);//set game to not accept input yet
        selfEnabled = true;

    }
    private void WelcomeError(System.Exception e)
    {
        WelcomeError(e.Message);
    }
    private void WelcomeError(string msg)
    {
        welcomeErrorText.text = msg;
    }
    public void Pause()
    {
        Pause(true);
    }
    public void Pause(bool cancel)
    {
        //brings up the pause menu

        io.CloseServos();//close servos if game is paused
        io.CloseSolenoid();//make sure the solenoid valve is not open
        StopAllCoroutines();//stop any game logic from continuing
        if(cancel) Results.CancelTrial();//throw away data about the current trial
        WaitForIR();// wait for the IR sensor to be tripped again before restarting the trial
        cm.Pause();// switch to pause menu
        selfEnabled = false;// do not continue with game logic until the user resumes the game
    }
    public void Resume()
    {
        StopAllCoroutines();// make sure no old coroutines resolve after we resume the trial
        cm.HUD();//switch back to the HUD canvas
        selfEnabled = true;// turn game logic checks back on
    }
    public void Quit()
    {
        //called by the quit button
        Application.Quit();
    }
    public void MainMenu()
    {
        //called by main menu buttons. Refresh the scene so that we are working with a clean slate
        io.Disconnect();
        SceneManager.LoadScene(0);
    }
    private void StartTrial()
    {
        //called after the IR sensor is tripped
        StopAllCoroutines();//stop any coroutines from resolving if applicable 
        SetState(true);//set game to accept input
        int side = ChooseSide();//choose side for grated circle to appear on
        gratedCircle.Reset(side);//move grated circle to that side
        Results.StartTrial(side,1);//tell the results object that a trial was started and where the stimulus appeared
        //opacity option is for if we want to be able to change the contrast of the stimulus
        StartCoroutine(WaitForTimeOut());//starts the check for timeout
    }
    private int ChooseSide()
    {
        //chooses the side that the stimulus should appear on based on mouse bias
        float leftBias = Results.LeftProportionOnInterval(6);
        float rand = Random.Range(0f,1f);
        if (rand > leftBias) return Globals.left;
        return Globals.right;
    }

    private void SetState(bool running)
    {
        //this function determines if the game is accepting input based on the running parameter

        if (running)
        {
            DisableForSeconds(camControl, controlPauseTime + io.EstimatedServoCloseTime());//causes control to be enabled after a pause
            StartCoroutine(WaitToOpenServos(controlPauseTime));//open the servos to allow the mouse to make a choice
        }
        else
        {
            camControl.enabled = false;//disable control of the camera via joystick
        }
        gratedCircle.gameObject.SetActive(running);//show the grated circle if the game is running
        targetRing.SetActive(running);

    }
    IEnumerator WaitToOpenServos(float time)
    {
        //allows a pause before opening the servos
        yield return new WaitForSeconds(time);
        io.OpenServos();
    }
    private void Success()
    {
        //called on successful completion of the trial
        StopAllCoroutines();//stop the time out from happening
        camControl.SnapTo(gratedCircle.GetWorldPos());//point the camera directly at the stimulus
        camControl.enabled = false;//prevent camera from being moved
        io.GiveWater();//reward the mouse
        io.CloseServos();
        Results.LogSuccess(io.ReadIR());//log the trial as a success. param determines if the mouse is in the viewport at time of completion
        DisableForSeconds(successPauseTime);//there should be a short pause on success during which the mouse can see the stimulus at the center of the screen
        StartCoroutine(WaitThenEndTrial(successPauseTime));
        sound.Success();//play the reward noise

    }
    private void Hit()
    {
        //called when the stimulus hits the edge of the screen

        Results.LogHit(io.ReadIR());//log trial as hit and record whether the mouse was in the viewport
        Fail();//call game fail logic
    }
    private void TimeOut()
    {
        //called when the mouse takes too long to respond
        bool irState = io.ReadIR();//check if mouse if in viewport
        Results.LogTimeOut(irState);//log timeout in results

        if (irState)
        {
            //if the mouse is there, show the fail screen, fail noise, etc
            Fail();
        }
        else
        {
            //if the mouse is not in viewport, just close the servos and end the trial (no noise/ black scree etc)
            io.CloseServos();
            EndTrial();
        }
    }
    public void TroubleshootHardware()//enters the troubleshooting scene
    {
        if(io!=null)   io.Disconnect();//disconnect from arduino
        welcomeFields.Save();//save current user input to the input fields
        SceneManager.LoadScene(1,0);//load the troubleshooting scene
    }
    private void Fail()//called when the mouse makes a wrong decision
    {
        StopAllCoroutines();//stop the timeout
        sound.Fail();//play the fail noise
        failPanel.SetActive(true);//show the black screen
        targetRing.SetActive(false);//dont show the target ring
        io.CloseServos();
        camControl.enabled = false;//disable control of the  camera
        DisableForSeconds(failPauseTime);//disable win/fail condition checking
        StartCoroutine(WaitThenEndTrial(failPauseTime));//end the trial after the time we want the mouse to see the fail screen
    }
    IEnumerator WaitThenEndTrial(float time)//ends the trial after some time
    {
        yield return new WaitForSeconds(time);
        EndTrial();
    }
    private void EndTrial()//ends the current trial
    {
        StopAllCoroutines();//stop the timeout coroutine
        SetState(false);//set game to not accept input
        if (Results.CurrentTrialNumber() >= numTrials)//have we completed all the trials?
        {
            EndGame();
            return;
        }
        failPanel.SetActive(false);//turn off the black screen panel
        WaitForIR();
    }
    private void EndGame()//called when the mouse completes all trials
    { 
        //convert time (seconds) into a string of HH:MM:SS, and update the pause menu title.
        int time = (int)(Time.time - startTime);
        int hours = time / 3600;
        time -= hours * 3600;
        int minutes = time / 60;
        int seconds = time - minutes * 60;
        Pause(false);//pause the game but dont cancel the current trial
        pauseMenuText.text = "Mouse " + SessionData.mouseID + " completed all trials in " + hours.ToString("00")+ ":" + minutes.ToString("00") + ":" +seconds.ToString("00");
        continueButton.SetActive(false);//remove the continue button so that the user can only go to the main menu
    }
    IEnumerator WaitForTimeOut()//times out the trial after the timeouttime passes
    {
        yield return new WaitForSeconds(timeOutTime);
        TimeOut();
    }
    private void DisableForSeconds(MonoBehaviour obj, float time)//disables a specific monobehavior for a set time
    {
        obj.enabled = false;
        StartCoroutine(EnableObject(obj, time));
    }
    IEnumerator EnableObject(MonoBehaviour obj, float time)// enables an object after some time
    {
        yield return new WaitForSeconds(time);
        obj.enabled = true;
    }
    private void DisableForSeconds(float time)//disables self for a set time
    {
        selfEnabled = false;
        StartCoroutine(EnableObject(time));
    }
    IEnumerator EnableObject(float time)//enables self after time
    {
        yield return new WaitForSeconds(time);
        selfEnabled = true;
    }
}
