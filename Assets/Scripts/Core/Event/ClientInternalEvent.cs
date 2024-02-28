using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine.Animations;

namespace XH
{
    // 221~320
    public struct DataEvent
    {
        public const short _DataEvent = 221;
        public const short ChangeLoginData = 222;
        public const short ChangeUserData = 223;
        public const short ChangeCreateRoomData = 224;
        public const short ChangeJoinRoomData = 225;
        public const short ChangeRoomData = 226;
        public const short ChangePlayerData = 227;
        public const short ChangeDingQueData = 228;
        public const short ChangeHandTileData = 229;
        public const short ChangeDrawTileData = 230;
        public const short ChangeThrowTileData = 231;
        public const short OtherThrowTileData = 232;
        public const short ChangePengGangReturnData = 233;
        public const short ChangeTipData = 234;
        public const short ChangeSelectTileData = 235;
        public const short ChangeButtonData = 236;
        public const short ChangeHuTileReturnData = 237;
    }
    // 331~430
    public struct UIEvent
    {
        public const short _ClientToInternal = 331;
        public const short RefreshLoginView = 332;
        public const short RefreshLobbyView = 333;
        public const short RefreshJoinRoomView = 334;
        public const short RefreshRoomView = 335;
        public const short ShowDiceAnim = 336;
        public const short ShowTimerAnim = 337;
        public const short RefreshPlayerView = 338;
        public const short RefreshQueTileType = 339;
        public const short RefreshHandTileView = 340;
        public const short RefreshDrawTileView = 341;
        public const short RefreshThrowTileView = 342;
        public const short RemoveThrowTile_1 = 343;
        public const short MoveHandTile = 344;
        public const short RefreshPengGangView = 345;
        public const short RefreshTipView = 346;
        public const short RefreshButtonView = 347;
        public const short RefreshHuTileView = 348;
    }
}
