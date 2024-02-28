using System.Collections.Generic;
using XH;

namespace mahjong
{
    struct SelectTileData
    {
        public bool isOnSelectTile;
        public ushort selectTileId;
        public List<ushort> selectTiles;

        public SelectTileData(ushort id)
        {
            isOnSelectTile = false;
            selectTileId = id;
            selectTiles = new List<ushort>();
        }
    }


    class SelectTileModel : Model
    {
        public override void Init(string name)
        {
            base.Init(name);
            ClientDataManager.Instance.AddData("SelectTileData", new SelectTileData(0));
        }

        public override void UnInit()
        {
            base.UnInit();
        }
    }
}
