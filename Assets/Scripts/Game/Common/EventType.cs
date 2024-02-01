using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XH
{

    public enum EventType
    {
        // Common
        LOGIC_ServerToClient,
        LOGIC_ClientToInternal,
        // LoginView
        LOGIC_Login,
        UI_ChangeLoginData,
        UI_RefreshLoginView,
        // LobbyView
        UI_ChangeUserData,
        UI_RefreshLobbyView,

        ButtonCreateRoom,
        ButtonJoinRoom,
        Test,
        SendPlayerToOther,
        SendLeaveRoomIdToOther,
        SendReadyPlayerIdToOther,
        SendTileToAll,
        SendSelectTileToAll,
        SendQueTileTypeToOther,
        SendPlayTileDataToOther,

        SendOtherPlayTileDataToAll,
        SendPengGangTileDataToAll,
        SendChiTileDataToAll,

        SendToNextDrawTile,

        SendDrawTileDataToOther,
        SendZiMoDataToAll,
        SendShangHuaDataToAll,
        SendAnGangJiaGangDataToAll,
        /// <summary>
        /// 在加杠这一步需要检测是否被抢杠
        /// </summary>
        SendQiangGangDataToOther,
        /// <summary>
        /// 抢杠胡的数据发给所有人
        /// </summary>
        SendQiangGangHuDataToAll,
        SendGameOverToAll,


    }
}
