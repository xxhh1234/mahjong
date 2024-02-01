using UnityEngine;
using UnityEngine.UI;

namespace XH
{
    public class PlayerProperties : MonoBehaviour
    {
        public int actorId = 0;
        public PlayerPosition playerPos = PlayerPosition.east;

        public Text txtUsername;
        public string username;
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
                txtUsername.text = username.ToString();
            }
        }

        public Text txtScore;
        public string score;
        public string Score
        {
            get
            {
                return Score;
            }
            set
            {
                score = value;
                if (actorId == 0) txtScore.text = "";
                else txtScore.text = score;
            }
        }

        public Image imgHead;
        public int sex = 1;
        public int Sex
        {
            get
            {
                return sex;
            }
            set
            {
                sex = value;
                if (actorId == 0) imgHead.sprite = GameResources.Instance.spriteDefault;
                else if (sex == 1)
                {
                    imgHead.sprite = GameResources.Instance.spriteBoy;
                }
                else if (sex == 2)
                {
                    imgHead.sprite = GameResources.Instance.spriteGirl;
                }
            }
        }

        public Image imgMaster;
        public bool isMaster = false;
        public bool IsMaster
        {
            get { return isMaster; }
            set { isMaster = value; imgMaster.gameObject.SetActive(isMaster); }
        }

        public Text txtQue;
        public string queTileType = "";
        public string QueTileType
        {
            get { return queTileType; }
            set
            {
                queTileType = value;
                if (queTileType == TileType.character.ToString()) txtQue.text = "Íò";
                else if (queTileType == TileType.bamboo.ToString()) txtQue.text = "Ìõ";
                else if (queTileType == TileType.circle.ToString()) txtQue.text = "Í²";
            }
        }

        public GameObject goRight;
        public bool isReady = false;
        public bool IsReady
        {
            get { return isReady; }
            set { isReady = value; goRight.SetActive(isReady); }
        }
    }
}
