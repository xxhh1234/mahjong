using System.Collections.Generic;
using TGClient;
using System;
using System.Threading.Tasks;

namespace XH
{
    public class Net : PeerBase
    {
        public override void OnOperationResponse(short opreationCode, ReceiveResponse response)
        {
            if(response.returnCode != 0)
                XHLogger.XH_ERROR((string)response.parameters[ParameterCode.error]);
            else
                EventManager.Instance.Broadcast(EventType.LOGIC_ServerToClient, (EventType)opreationCode, response.parameters);

            /*
            OpCode opcode = (OpCode)opreationCode;
            // 请求成功
            if (response.returnCode == 0)
            {
                switch (opcode)
                {
                    case OpCode.Test:
                    break;
                    case OpCode.Login:
                    {
                        userInfo = Convert<UserInfo>(response.parameters[ParameterCode.userInfo]);
                        GameManager.Instance.Login(userInfo);
                    }
                    break;
                    case OpCode.CreateRoomPanel:
                    {
                        Player player = Convert<Player>(response.parameters[ParameterCode.player]);
                        RoomInfo roomInfo = Convert<RoomInfo>(response.parameters[ParameterCode.roomInfo]);
                        GameManager.Instance.CreateRoomPanel(player, roomInfo);
                    }
                    break;
                    case OpCode.JoinRoomPanel:
                    {
                        Player player = Convert<Player>(response.parameters[ParameterCode.player]);
                        RoomInfo roomInfo = Convert<RoomInfo>(response.parameters[ParameterCode.roomInfo]);
                        List<Player> playerList = Convert<List<Player>>(response.parameters[ParameterCode.playerList]);
                        GameManager.Instance.JoinRoomPanel(player, roomInfo, playerList);
                    }
                    break;
                    case OpCode.LeaveRoomPanel:
                    {
                        GameManager.Instance.LeaveRoomPanel();
                    }
                    break;
                    case OpCode.ClickReady:
                    {
                        GameManager.Instance.ClickReady();
                    }
                    break;
                    case OpCode.DrawTile:
                    {
                        int remainTile = Convert<int>(response.parameters[ParameterCode.remainTile]);
                        int drawTileNum = Convert<int>(response.parameters[ParameterCode.drawTileNum]);
                        bool isGangHouDrawTile = Convert<bool>(response.parameters[ParameterCode.isGangHouDrawTile]);
                        ZiMoType ziMoType = Convert<ZiMoType>(response.parameters[ParameterCode.ziMoType]);
                        ShangHuaType ShangHuaType = Convert<ShangHuaType>(response.parameters[ParameterCode.shangHuaType]);
                        PengGangType anGangJiaGangType = Convert<PengGangType>(response.parameters[ParameterCode.anGangJiaGangType]);
                        GameManager.Instance.DrawTileData(remainTile, drawTileNum, isGangHouDrawTile,
                                                               ziMoType, ShangHuaType, anGangJiaGangType);
                    }
                    break;
                    case OpCode.GoOn:
                    {
                        GameManager.Instance.InitRoom();
                    }
                    break;
                    default:
                    break;
                }
            }
            // 请求失败
            else
            {
                string error = (string)response.parameters[ParameterCode.error];
                Debug.Log(error);
            }

        }

        public T Convert<T>(object o)
        {
            string str = JsonConvert.SerializeObject(o);
            T t = JsonConvert.DeserializeObject<T>(str);
            return t;
        }
        */
        }
        public override void OnEvent(short eventCode, Dictionary<short, object> dict)
        {
            EventManager.Instance.Broadcast(EventType.LOGIC_ServerToClient, (EventType)eventCode, dict);
            /*
            EventCode evCode = (EventCode)eventCode;
            switch (evCode)
            {
                case EventCode.Test:
                break;
                case EventCode.SendPlayerToOther:
                {
                    Player otherPlayer = Convert<Player>(dict[ParameterCode.player]);
                    RoomInfo RoomInfo = Convert<RoomInfo>(dict[ParameterCode.roomInfo]);
                    GameManager.Instance.OtherJoinRoomPanel(otherPlayer, RoomInfo);
                }
                break;
                case EventCode.SendLeaveRoomIdToOther:
                {
                    int leaveId = Convert<int>(dict[ParameterCode.actorId]);
                    GameManager.Instance.OtherLeaveRoomPanel(leaveId);
                }
                break;
                case EventCode.SendReadyPlayerIdToOther:
                {
                    int readyId = Convert<int>(dict[ParameterCode.actorId]);
                    GameManager.Instance.OtherClickReady(readyId);
                }
                break;
                case EventCode.SendTileToAll:
                {
                    int remainTile = Convert<int>(dict[ParameterCode.remainTile]);
                    List<int> handTile = Convert<List<int>>(dict[ParameterCode.handTile]);
                    GameManager.Instance.SendTile(remainTile, handTile);
                }
                break;
                case EventCode.SendSelectTileToAll:
                {
                    List<int> selectTileList = Convert<List<int>>(dict[ParameterCode.selectTileList]);
                    GameManager.Instance.AddThreeTile(selectTileList);
                }
                break;
                case EventCode.SendQueTileTypeToOther:
                {
                    TileType queTileType = Convert<TileType>(dict[ParameterCode.queTileType]);
                    int queId = Convert<int>(dict[ParameterCode.actorId]);
                    GameManager.Instance.OtherDingQue(queTileType, queId);
                }
                break;
                case EventCode.SendPlayTileDataToOther:
                {
                    int playTileNum = Convert<int>(dict[ParameterCode.playTileNum]);
                    int playTileId = Convert<int>(dict[ParameterCode.playTileId]);
                    OtherPlayTileType otherPlayTileType = Convert<OtherPlayTileType>(dict[ParameterCode.otherPlayTileType]);
                    PengGangType pengGangType = Convert<PengGangType>(dict[ParameterCode.PengGangType]);
                    GameManager.Instance.PlayTileData(playTileNum, playTileId, otherPlayTileType, pengGangType);
                    //  如果不胡不碰不杠，直接发送回复
                    if (otherPlayTileType.Equals(OtherPlayTileType.buhu) && pengGangType.Equals(PengGangType.none))
                    {
                        Dictionary<short, object> dictPlayTileDataReturn = new Dictionary<short, object>();
                        dictPlayTileDataReturn.Add(ParameterCode.none, null);
                        SendRequest((short)OpCode.PlayTileDataReturn, dictPlayTileDataReturn);
                    }
                }
                break;
                case EventCode.SendOtherPlayTileDataToAll:
                {
                    int playTileNum = Convert<int>(dict[ParameterCode.playTileNum]);
                    int playTileId = Convert<int>(dict[ParameterCode.playTileId]);
                    List<OtherPlayTileReturnData> otherPlayTileDataList =
                        Convert<List<OtherPlayTileReturnData>>(dict[ParameterCode.otherPlayTileDataList]);
                    List<List<int>> huHandTileList = Convert<List<List<int>>>(dict[ParameterCode.handTileList]);
                    List<int> scoreList = Convert<List<int>>(dict[ParameterCode.scoreList]);
                    GameManager.Instance.OtherPlayTileData(playTileNum, playTileId, otherPlayTileDataList,
                                                               huHandTileList, scoreList);
                }
                break;
                case EventCode.SendPengGangTileDataToAll:
                {
                    int playTileNum = Convert<int>(dict[ParameterCode.playTileNum]);
                    int pengGangId = Convert<int>(dict[ParameterCode.pengGangId]);
                    PengGangType PengGangType = Convert<PengGangType>(dict[ParameterCode.PengGangType]);
                    GameManager.Instance.PengGangTileData(playTileNum, pengGangId, PengGangType);
                }
                break;
                case EventCode.SendToNextDrawTile:
                {
                    SendRequest((short)OpCode.DrawTile, dict);
                }
                break;
                case EventCode.SendDrawTileDataToOther:
                {
                    int remainTile = Convert<int>(dict[ParameterCode.remainTile]);
                    int drawTileId = Convert<int>(dict[ParameterCode.drawTileId]);
                    GameManager.Instance.OtherDrawTileData(remainTile, drawTileId);
                }
                break;
                case EventCode.SendZiMoDataToAll:
                {
                    ZiMoType ziMoType = Convert<ZiMoType>(dict[ParameterCode.ziMoType]);
                    int ziMoId = Convert<int>(dict[ParameterCode.ziMoId]);
                    int ziMoNum = Convert<int>(dict[ParameterCode.ziMoType]);
                    List<int> ziMoHandTile = Convert<List<int>>(dict[ParameterCode.handTile]);
                    List<int> scoreList = Convert<List<int>>(dict[ParameterCode.scoreList]);
                    GameManager.Instance.ZiMoData(ziMoType, ziMoId, ziMoNum, ziMoHandTile, scoreList);
                }
                break;
                case EventCode.SendShangHuaDataToAll:
                {
                    ShangHuaType shangHuaType = Convert<ShangHuaType>(dict[ParameterCode.shangHuaType]);
                    int shangHuaId = Convert<int>(dict[ParameterCode.shangHuaId]);
                    int shangHuaNum = Convert<int>(dict[ParameterCode.shangHuaNum]);
                    List<int> shangHuaHandTile = Convert<List<int>>(dict[ParameterCode.handTile]);
                    List<int> scoreList = Convert<List<int>>(dict[ParameterCode.scoreList]);
                    GameManager.Instance.ShangHuaData(shangHuaType, shangHuaId, shangHuaNum, shangHuaHandTile, scoreList);
                }
                break;
                case EventCode.SendAnGangJiaGangDataToAll:
                {
                    PengGangType anGangJiaGangType = Convert<PengGangType>(dict[ParameterCode.anGangJiaGangType]);
                    int anGangJiaGangId = Convert<int>(dict[ParameterCode.anGangJiaGangId]);
                    int anGangJiaGangNum = Convert<int>(dict[ParameterCode.anGangJiaGangNum]);
                    GameManager.Instance.AnGangJiaGangData(anGangJiaGangType, anGangJiaGangId, anGangJiaGangNum);
                }
                break;
                case EventCode.SendQiangGangDataToOther:
                {
                    OtherPlayTileType otherPlayTileType = Convert<OtherPlayTileType>(dict[ParameterCode.otherPlayTileType]);
                    GameManager.Instance.QiangGangData(otherPlayTileType);
                }
                break;
                case EventCode.SendQiangGangHuDataToAll:
                {
                    List<QiangGangReturnData> qiangGangHuDataList =
                        Convert<List<QiangGangReturnData>>(dict[ParameterCode.qiangGangHuDataList]);
                    List<List<int>> huHandTileList = Convert<List<List<int>>>(dict[ParameterCode.handTileList]);
                    List<int> scoreList = Convert<List<int>>(dict[ParameterCode.scoreList]);
                    GameManager.Instance.QiangGangDataReturn(qiangGangHuDataList, huHandTileList, scoreList);
                }
                break;
                case EventCode.SendGameOverToAll:
                {
                    List<List<int>> noHuHandTileList = Convert<List<List<int>>>(dict[ParameterCode.handTileList]);
                    List<int> noHuPlayerIdList = Convert<List<int>>(dict[ParameterCode.noHuPlayerIdList]);
                    List<int> scoreList = Convert<List<int>>(dict[ParameterCode.scoreList]);
                    GameManager.Instance.GameOver(noHuHandTileList, noHuPlayerIdList, scoreList);
                }
                break;
                default:
                break;
            
            }
            */
        }
        
        public override void OnConnected(string message)
        {
            XHLogger.XH_LOG(message);
        }
        public override void OnDisConnect(Exception connectException)
        {
            XHLogger.XH_LOG(connectException.ToString());
        }
        public override void OnException(Exception exception)
        {
            XHLogger.XH_LOG(exception.ToString());
        }

        public async void SendRequestAsync(short opCode, Dictionary<short, object> opDict, 
            Action<Dictionary<short, object>> action=null)
        {
            await Task.Run(() =>
            {
                if (action != null)
                    EventManager.Instance.AddListener((EventType)opCode, (Dictionary<short, object> respDict) =>
                    {
                        ThreadManager.ExecuteUpdate(() => { action(respDict);});
                    });
                SendRequest(opCode, opDict);
            });           
        }
        public void SendRequestSync(short opCode, Dictionary<short, object> opDict,
            Action<Dictionary<short, object>> action = null)
        {
            if (action != null)
                EventManager.Instance.AddListener((EventType)opCode, (Dictionary<short, object> respDict) =>
                {
                    ThreadManager.ExecuteUpdate(() => { action(respDict); });
                });
            SendRequest(opCode, opDict);
        }
    }
}