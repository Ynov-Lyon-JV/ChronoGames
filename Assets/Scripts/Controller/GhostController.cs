using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    #region Fields

    private GhostData datas;
    private GameManager gm;

    #endregion

    IEnumerator LerpPosition()
    {
        for (int i = 0; i < datas.Positions.Count; i++)
        {
            float time = 0;
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;

            Vector3 pos = datas.Positions[i];
            Quaternion rot = datas.Rotations[i];

            while (time < (1 / gm.Frequence))
            {
                transform.position = Vector3.Lerp(startPos, pos, 1 - Mathf.Exp(-20 * Time.deltaTime)); //1 - Mathf.Exp(-20 * Time.deltaTime)
                transform.rotation = Quaternion.Lerp(startRot, rot, 1 - Mathf.Exp(-20 * Time.deltaTime));

                time += Time.deltaTime;
                yield return null;
            }
            //transform.position = pos;
            //transform.rotation = rot;
        }
    }

    #region Unity Methods
    void Start()
    {
        gm = FindObjectOfType<GameManager>();

        StartCoroutine(LerpPosition());
    }

    // Update is called once per frame
    void Update()
    {

    }

    #endregion

    #region Public Methods

    public void SetDatas(GhostData _datas)
    {
        datas = _datas;
    }

    #endregion
}
