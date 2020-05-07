 /* Developed by: Jorge Ignacio Pereira Suárez
 * email: joignaciosuarez@gmail.com
 * gitHub: https://github.com/JoPereriaSuarez
 */

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0649

/// <summary>
/// Class that represent the Table to display
/// </summary>
public class TableManager : MonoBehaviour
{
    /// <summary>
     /// Json key to identify the title of the table
     /// </summary>
    private const string KEY_TITLE = "Title";

    /// <summary>
    /// Json key to identify the headers of the table
    /// </summary>
    private const string KEY_HEADERS = "ColumnHeaders";

    /// <summary>
    /// Json key to identify the data of the table
    /// </summary>
    private const string KEY_DATA = "Data";


    #region UNITY INSPECTOR VARIABLES

    /// <summary>
    /// Button to update the table
    /// </summary>
    [Tooltip("Button to update the table")]
    [SerializeField]
    private Button updateButton;

    /// <summary>
    /// UI Text with for title reference
    /// </summary>
    [Tooltip("UI Text with for title reference")]
    [SerializeField]
    private Text titleText;

    /// <summary>
    /// Prefab representing one column of the table
    /// </summary>
    [Tooltip("Prefab representing one column of the table")]
    [SerializeField]
    private GameObject columnPrefab;

    /// <summary>
    /// Prefab representing one data of the table
    /// </summary>
    [Tooltip("Prefab representing one data of the table")]
    [SerializeField]
    private GameObject dataPrefab;

    /// <summary>
    /// Parent to create the columns
    /// </summary>
    [Tooltip("Parent to create the columns")]
    [SerializeField]
    private Transform columnsParent;

    #endregion

    #region INTERNAL VARIABLES

    /// <summary>
    /// Title of this table
    /// </summary>
    private string _titleValue = null;

    /// <summary>
    /// List of column headers
    /// </summary>
    private List<string> _headersValue = new List<string>();

    /// <summary>
    /// List of data to populate the table
    /// </summary>
    private List<string[]> _dataValues = new List<string[]>();

    #endregion

    #region MONOBEHAVIOUR' METHODS

    private void Awake()
    {
        updateButton.onClick.AddListener(() => UpdateUiTable());
    }

    private void Start()
    {
        UpdateUiTable();
    }

    private void OnDestroy()
    {
        updateButton?.onClick.RemoveAllListeners();
    }
    #endregion

    #region INTERNAL METHODS

    /// <summary>
    /// Method that update the UI elements of the table
    /// </summary>
    private void UpdateUiTable()
    {
        ClearTable();
        UpdateTableData();

        titleText.text = _titleValue;

        GameObject temp_columnInstance;
        Text temp_tex;
        RectTransform temp_rectTransform;
        for (int i = 0; i < _headersValue.Count; i++)
        {
            temp_columnInstance = Instantiate(columnPrefab, columnsParent);
            temp_tex = temp_columnInstance.GetComponent<Text>();
            temp_tex.text = _headersValue[i];

            temp_rectTransform = temp_columnInstance.GetComponent<RectTransform>();

            List<string[]> matches_data = _dataValues.FindAll((data) => data[0] == _headersValue[i]);

            for (int j = 0; j < matches_data.Count; j++)
            {
                //GameObject temp_dataInstance = new GameObject("Data", typeof(Text));
                GameObject temp_dataInstance = Instantiate(dataPrefab, temp_columnInstance.transform);
                temp_tex = temp_dataInstance.GetComponent<Text>();
                temp_tex.text = matches_data[j][1];
                temp_dataInstance.transform.parent = temp_columnInstance.transform;
                _dataValues.Remove(matches_data[j]);
            }
        }
    }

    /// <summary>
    /// Method that update the table data from the json file
    /// </summary>
    /// <returns></returns>
    private void UpdateTableData()
    {
        _dataValues.Clear();
        _dataValues.TrimExcess();


        string jsonString = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "JsonChallenge.json"));
        JObject jsonObject = JObject.Parse(jsonString);
        JArray jsonArray;
        foreach (KeyValuePair<string, JToken> content in jsonObject)
        {
            switch (content.Key)
            {
                case KEY_TITLE:
                    _titleValue = content.Value.ToString();
                    break;
                case KEY_HEADERS:
                    jsonArray = JArray.Parse(content.Value.ToString());
                    foreach (var item in jsonArray.Children())
                    {
                        _headersValue.Add(item.ToString());
                    }
                    break;
                case KEY_DATA:
                    jsonArray = JArray.Parse(content.Value.ToString());
                    foreach (var item in jsonArray.Children())
                    {
                        foreach(var prop in item.Children<JProperty>())
                        {
                            _dataValues.Add(new string[] { prop.Name, prop.Value.ToString() });
                        }
                    }
                    break;
                default:
                    break;
            }
        }

    }

    /// <summary>
    /// Method that deletes all columnsObject instances
    /// </summary>
    private void ClearTable()
    {
        for (int i = 0; i < columnsParent.childCount; i++)
        {
            Destroy(columnsParent.GetChild(i).gameObject);
        }

        _titleValue = "";
        _headersValue.Clear();
        _headersValue.TrimExcess();
        _dataValues.Clear();
        _dataValues.TrimExcess();
    }
    #endregion
}

