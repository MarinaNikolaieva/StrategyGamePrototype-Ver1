using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ErrorPanelActionOnOK : MonoBehaviour
{
    public GeneratorGlobalData generatorGlobalData;
    public GlobalDataContainer UIControlData;
    public GlobalDataController UIControllerData;
    public GameObject ErrorPanel;
    public GameObject GameplayErrorPanel;
    private GameObject CallerPanel;

    public void SetCallerPanel(GameObject callerPanel)
    {
        CallerPanel = callerPanel;
    }

    //NEEDED on success, forward the user to a different panels

    private void QuitNoSavingAction()
    {

    }

    private void DeleteSaveFileAction()
    {

    }

    private void OverwriteSaveFileAction()
    {
        //In here, we expect the SavePanel
        SaveAndLoad saveAndLoad = new SaveAndLoad();
        saveAndLoad.generatorGlobalData = generatorGlobalData;
        saveAndLoad.ErrorPanel = ErrorPanel;
        saveAndLoad.CallerPanel = CallerPanel;
        saveAndLoad.globalDataContainer = UIControlData;
        saveAndLoad.Save();
        saveAndLoad.SaveTotal();
    }

    private void LoadSaveFileNoSavingAction()
    {
        //Nere, we expect the LoadPanel
        SaveAndLoad saveAndLoad = new SaveAndLoad();
        saveAndLoad.generatorGlobalData = generatorGlobalData;
        saveAndLoad.ErrorPanel = ErrorPanel;
        saveAndLoad.CallerPanel = CallerPanel;
        saveAndLoad.globalDataContainer = UIControlData;
        saveAndLoad.Load("LoadName");
        saveAndLoad.LoadTotal("LoadName", UIControllerData);
        //Load economics as well
    }

    private void RestartWorldConfirmAction()
    {
        //Nere, we expect the RestartWorldPanel
        SaveAndLoad saveAndLoad = new SaveAndLoad();
        saveAndLoad.generatorGlobalData = generatorGlobalData;
        saveAndLoad.ErrorPanel = ErrorPanel;
        saveAndLoad.CallerPanel = CallerPanel;
        saveAndLoad.globalDataContainer = UIControlData;
        saveAndLoad.Load("RestartName");
    }

    private void ConfirmDeleteObjectAction()
    {
        UIControlData.actionValid = true;
    }


    public void OKButtonClick()
    {
        string callerPanelName = CallerPanel.name;
        if (callerPanelName.Contains("Clone"))
            callerPanelName = callerPanelName.Substring(0, callerPanelName.Length - 7);
        switch (callerPanelName)
        {
            case "SavePanel":
                OverwriteSaveFileAction();
                break;
            case "LoadPanel":
                LoadSaveFileNoSavingAction();
                break;
            case "RestartWorldPanel":
                RestartWorldConfirmAction();
                break;
            case "EStorageRemove":
                ConfirmDeleteObjectAction();
                break;
            case "EFacilityRemove":
                ConfirmDeleteObjectAction();
                break;
            case "LPathwayRemove":
                ConfirmDeleteObjectAction();
                break;
        }
    }
}
