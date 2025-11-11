namespace Game.Server.Packets
{
    public enum SanXiaoPackageType
    {
        CREATE_MAP = 1,
        REQUIRE_MAP = 2,
        BOOM = 3,
        FILL_ARR = 4,
        DATA = 5,
        IS_OPEN = 6,
        GAIN_PRISE = 7,
        BUY_ITEM = 8,
        HITS_END = 9,
        BUY_TIMES = 15,
        REWARDS_DATA = 17,
        STORE_DATA = 18,
        DROP_OUT_GAINED_ITEM = 19,
        PROP_CROSS_BOMB = 41,
        PROP_SQUARE_BOMB = 48,
        PROP_CLEAR_COLOR = 49,
        PROP_CHANGE_COLOR = 50,
    }
}