﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wo.KancolleCore.KancolleNavigation;
using Wo.Utils;

namespace Wo.KancolleCore
{
    /// <summary>
    /// 游戏数据
    /// </summary>
    public class KancolleGameData
    {

        private static KancolleGameData s_instance = null;

        /// <summary>
        /// 游戏数据单例实例
        /// </summary>
        /// <returns></returns>
        public static KancolleGameData Instance { get { return s_instance; } }

        /// <summary>
        /// 舰娘类型字典
        /// key: 舰娘类型ID
        /// value: 舰娘类型信息
        /// </summary>
        public ReadOnlyDictionary<int, KancolleShipType> ShipTypeDictionary { get; internal set; } = new ReadOnlyDictionary<int, KancolleShipType>(new Dictionary<int, KancolleShipType>());

        /// <summary>
        /// 装备类型字典
        /// key: 装备类型ID
        /// value: 装备类型信息
        /// </summary>
        public ReadOnlyDictionary<int, KancolleItemEquipType> ItemEquipTypeDictionary { get; internal set; } = new ReadOnlyDictionary<int, KancolleItemEquipType>(new Dictionary<int, KancolleItemEquipType>());

        /// <summary>
        /// 海域信息字典
        /// key: 海域ID
        /// value: 海域信息
        /// </summary>
        public ReadOnlyDictionary<int, KancolleMapInfoData> MapInfoDictionary { get; internal set; } = new ReadOnlyDictionary<int, KancolleMapInfoData>(new Dictionary<int, KancolleMapInfoData>());

        /// <summary>
        /// 基本信息
        /// </summary>
        public KancolleBasicInfo BasicInfo { get; internal set; } = new KancolleBasicInfo();

        /// <summary>
        /// 任务字典
        /// key: 任务ID
        /// value: 任务信息
        /// </summary>
        public ReadOnlyDictionary<int, KancolleQuest> QuestDictionary { get; internal set; } = new ReadOnlyDictionary<int, KancolleQuest>(new Dictionary<int, KancolleQuest>());


        /// <summary>
        /// 当前资源
        /// </summary>
        public KancolleMaterial Material { get; internal set; } = new KancolleMaterial();

        /// <summary>
        /// 舰娘（数据库）信息字典
        /// key: 舰娘ID
        /// value: 舰娘（数据库）信息
        /// </summary>
        public ReadOnlyDictionary<int, KancolleShipData> ShipDataDictionary { get; internal set; } = new ReadOnlyDictionary<int, KancolleShipData>(new Dictionary<int, KancolleShipData>());

        /// <summary>
        /// 拥有的舰娘信息字典
        /// key: 舰娘No
        /// value: 舰娘信息
        /// </summary>
        public ReadOnlyDictionary<int, KancolleShip> OwnedShipDictionary { get; internal set; } = new ReadOnlyDictionary<int, KancolleShip>(new Dictionary<int, KancolleShip>());

        /// <summary>
        /// 远征信息（数据库）字典
        /// key: 远征ID
        /// value: 远征（数据库）信息
        /// </summary>
        public ReadOnlyDictionary<int, KancolleMissionData> MissionDictionary { get; internal set; } = new ReadOnlyDictionary<int, KancolleMissionData>(new Dictionary<int, KancolleMissionData>());

        /// <summary>
        /// 正在跑的远征信息字典
        /// key: 远征ID
        /// value: 远征信息
        /// </summary>
        public ReadOnlyDictionary<int, KancolleMissson> OwnedMissionDic { get; internal set; } = new ReadOnlyDictionary<int, KancolleMissson>(new Dictionary<int, KancolleMissson>());

        /// <summary>
        /// 舰娘的位置字典，用于双向查询舰娘的位置
        /// key: 舰娘No
        /// value: 舰娘位置，Tuple(i,j),表示第i个舰队，第j个位置，从0开始算
        /// </summary>
        public ReadOnlyDictionary<int, Tuple<int, int>> OwnedShipPlaceDictionary { get; internal set; } = new ReadOnlyDictionary<int, Tuple<int, int>>(new Dictionary<int, Tuple<int, int>>());

        /// <summary>
        /// 位置为[i,j]的舰娘，用于双向查询舰娘的位置
        /// 索引[i,j],表示第i个舰队，第j个位置，从0开始算
        /// 返回的是再[i,j]的舰娘No
        /// </summary>
        public ReadOnlyArray2<int> OwnedShipPlaceArray { get; internal set; } = new ReadOnlyArray2<int>(new int[4, 6]);

        /// <summary>
        /// 装备（数据库）字典
        /// key: 装备ID
        /// value: 装备（数据库）信息
        /// </summary>
        public ReadOnlyDictionary<int, KancolleSlotItemData> SlotDictionary { get; internal set; } = new ReadOnlyDictionary<int, KancolleSlotItemData>(new Dictionary<int, KancolleSlotItemData>());

        /// <summary>
        /// 拥有的装备字典
        /// key: 装备NO
        /// value: 装备信息
        /// </summary>
        public ReadOnlyDictionary<int, KancolleSlotItem> OwnedSlotDictionary { get; internal set; } = new ReadOnlyDictionary<int, KancolleSlotItem>(new Dictionary<int, KancolleSlotItem>());

        /// <summary>
        /// 已经装备的装备字典
        /// key:装备NO
        /// value:装备的舰娘NO
        /// </summary>
        public ReadOnlyDictionary<int, int> EquipedSlotDictionary { get; internal set; } = new ReadOnlyDictionary<int, int>(new Dictionary<int, int>());

        /// <summary>
        /// 尚未装备的装备列表，每个元素为装备NO
        /// </summary>
        public ReadOnlyCollection<int> UnEquipedSlotArray { get; internal set; } = new ReadOnlyCollection<int>(new List<int>());

        /// <summary>
        /// 修理渠信息
        /// </summary>
        public ReadOnlyCollection<KancolleDockData> DockArray { get; internal set; } = new ReadOnlyCollection<KancolleDockData>(new KancolleDockData[0]);

        /// <summary>
        /// 当前场景
        /// </summary>
        public KancolleScene CurrentScene { get; internal set; }

        public KancolleGameData()
        {
            Debug.Assert(s_instance == null);
            s_instance = this;
        }

        #region public methods
        /// <summary>
        /// 从ownedShip获得数据库的shipData，若不存在返回null
        /// </summary>
        /// <param name="ownedShip"></param>
        /// <returns></returns>
        public KancolleShipData GetShip(KancolleShip ownedShip)
        {
            KancolleShipData shipData;
            if (ShipDataDictionary.TryGetValue(ownedShip.No, out shipData))
                return shipData;
            return null;
        }

        /// <summary>
        /// 从ownedShipId获得数据库的shipData，若不存在返回null
        /// </summary>
        /// <param name="ownedShipNo"></param>
        /// <returns></returns>
        public KancolleShipData GetShip(int ownedShipNo)
        {
            KancolleShip ownedShip;
            if (OwnedShipDictionary.TryGetValue(ownedShipNo, out ownedShip))
                return GetShip(ownedShip);
            return null;
        }

        /// <summary>
        /// 获取舰娘类型id，若不存在返回-1
        /// </summary>
        /// <param name="ownedShip"></param>
        /// <returns></returns>
        public int GetShipType(KancolleShip ownedShip)
        {
            KancolleShipData shipData;
            if (ShipDataDictionary.TryGetValue(ownedShip.No, out shipData))
                return shipData.Type;
            return -1;
        }

        /// <summary>
        /// 获取舰娘类型id，若不存在返回-1
        /// </summary>
        /// <param name="ownedShipNo"></param>
        /// <returns></returns>
        public int GetShipType(int ownedShipNo)
        {
            KancolleShip ownedShip;
            if (OwnedShipDictionary.TryGetValue(ownedShipNo, out ownedShip))
                return GetShipType(ownedShip);
            return -1;
        }

        /// <summary>
        /// 获取舰娘名，若不存在返回null
        /// </summary>
        /// <param name="ownedShip"></param>
        /// <returns></returns>
        public string GetShipName(KancolleShip ownedShip)
        {
            KancolleShipData shipData;
            if (ShipDataDictionary.TryGetValue(ownedShip.ShipId, out shipData))
                return shipData.Name;
            return null;
        }

        /// <summary>
        /// 获取舰娘名，若不存在返回null
        /// </summary>
        /// <param name="ownedShipNo"></param>
        /// <returns></returns>
        public string GetShipName(int ownedShipNo)
        {
            KancolleShip ownedShip;
            if (OwnedShipDictionary.TryGetValue(ownedShipNo, out ownedShip))
                return GetShipName(ownedShip);
            return null;
        }

        /// <summary>
        /// 获取装备名，若不存在则返回null
        /// </summary>
        /// <param name="ownedItem"></param>
        /// <returns></returns>
        public string GetSlotItemName(KancolleSlotItem ownedItem)
        {
            KancolleSlotItemData itemData;
            if (SlotDictionary.TryGetValue(ownedItem.SlotItemId, out itemData))
                return itemData.Name;
            return null;
        }

        /// <summary>
        /// 获取装备名，若不存在则返回null
        /// </summary>
        /// <param name="ownedItemNo"></param>
        /// <returns></returns>
        public string GetSlotItemName(int ownedItemNo)
        {
            KancolleSlotItem item;
            if (OwnedSlotDictionary.TryGetValue(ownedItemNo, out item))
                return GetSlotItemName(item);
            return null;
        }

        /// <summary>
        /// 舰娘能否佩戴该装备，可以返回true，否则返回false；舰娘、装备没有找到都返回false
        /// </summary>
        /// <param name="shipId">舰娘id</param>
        /// <param name="itemId">装备id</param>
        /// <returns></returns>
        public bool CanShipEquipItem(int shipId, int itemId)
        {
            KancolleShipData ship;
            if (ShipDataDictionary.TryGetValue(shipId, out ship))
            {
                KancolleShipType shipType;
                //找到舰娘类型
                if (ShipTypeDictionary.TryGetValue(ship.Type, out shipType))
                {
                    //舰娘的可装备字典
                    var dic = shipType.EquipItemType;
                    KancolleSlotItemData item;
                    //找到装备
                    if (SlotDictionary.TryGetValue(itemId, out item))
                    {
                        //item3为该装备的装备类型                  
                        if (dic.ContainsKey(item.Type.Item3))
                        {
                            //可装备（1）则为true，否则为false
                            return dic[item.Type.Item3] != 0;
                        }

                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 舰娘能否佩戴该装备，可以返回true，否则返回false；舰娘、装备为null或者没有找到都返回false
        /// </summary>
        /// <param name="ship"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CanShipEquipItem(KancolleShipData ship, KancolleSlotItemData item)
        {
            if (ship == null || item == null)
                return false;
            return CanShipEquipItem(ship.ShipId, item.Id);
        }

        /// <summary>
        /// 舰娘是否已经入渠
        /// </summary>
        /// <param name="shipNo"></param>
        /// <returns></returns>
        public bool IsShipRepairing(int shipNo)
        {
            for (int i = 0; i < DockArray.Count; i++)
            {
                if (DockArray[i].State > 0 && DockArray[i].ShipId == shipNo)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 舰娘是否已经入渠
        /// </summary>
        /// <param name="ship"></param>
        /// <returns></returns>
        public bool IsShipRepairing(KancolleShip ship)
        {
            return IsShipRepairing(ship.No);
        }


        /// <summary>
        /// 是否有大破船只
        /// </summary>
        /// <param name="deckNo">舰队号，从0开始,0-3</param>
        /// <returns></returns>
        public bool HasBigBrokenShip(int deckNo)
        {
            for (int j = 0; j < OwnedShipPlaceArray.ColumnCount; j++)
            {
                var shipNo = OwnedShipPlaceArray[deckNo, j];
                if (shipNo > 0 && OwnedShipDictionary[shipNo].BigBroken)
                {
                    return true;
                }
            }

            //如果存在战斗，则预测战斗***********
            /*if (Battle.CurrentBattle != null)
            {
                foreach (var ship in Battle.CurrentBattle.MainFleet)
                {
                    if (ship != null && KancolleShip.IsBigBroken(ship.ToHP, ship.MaxHP))
                        return true;
                }
                if (Battle.CurrentBattle.EscortFleet != null)
                {
                    foreach (var ship in Battle.CurrentBattle.EscortFleet)
                    {
                        if (ship != null && KancolleShip.IsBigBroken(ship.ToHP, ship.MaxHP))
                            return true;
                    }
                }
            }*/

            return false;
        }
        #endregion

    }

    /// <summary>
    /// 提督基本信息
    /// </summary>
    public class KancolleBasicInfo
    {
        /// <summary>
        /// 提督等级
        /// </summary>
        public int Level { get; internal set; }

        /// <summary>
        /// 头衔id
        /// </summary>
        public int RankId { get; internal set; }

        /// <summary>
        /// 头衔
        /// </summary>
        public string Rank { get; internal set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; internal set; }

        /// <summary>
        /// 拥有的舰娘个数
        /// </summary>
        public int OwnedShipCount { get; internal set; }

        /// <summary>
        /// 最大可保有舰娘个数
        /// </summary>
        public int MaxShipCount { get; internal set; }

        /// <summary>
        /// 最大可保有装备个数
        /// </summary>
        public int MaxSlotItemCount { get; internal set; }

        internal KancolleBasicInfo() { }

        internal KancolleBasicInfo(JToken api_port_data)
        {
            Level = api_port_data["api_basic"]["api_level"].ToObject<int>();
            RankId = api_port_data["api_basic"]["api_rank"].ToObject<int>();
            Rank = KancolleUtils.RankText[api_port_data["api_basic"]["api_rank"].ToObject<int>()];
            NickName = api_port_data["api_basic"]["api_nickname"].ToString();
            OwnedShipCount = api_port_data["api_ship"].ToArray().Length;
            MaxShipCount = api_port_data["api_basic"]["api_max_chara"].ToObject<int>();
            MaxSlotItemCount = api_port_data["api_basic"]["api_max_slotitem"].ToObject<int>();
        }
    }

    /// <summary>
    /// 舰娘信息
    /// </summary>
    public class KancolleShip
    {
        /// <summary>
        /// OwnedNo，按照船的获得顺序生成
        /// </summary>
        public int No { get; internal set; }

        /// <summary>
        /// 数据库中的船的id
        /// </summary>
        public int ShipId { get; internal set; }

        /// <summary>
        /// 等级
        /// </summary>
        public int Level { get; internal set; }

        /// <summary>
        /// 疲劳值
        /// </summary>
        public int Condition { get; internal set; }

        /// <summary>
        /// 火力（当前值，最大值）
        /// </summary>
        public Tuple<int, int> Karyoku { get; internal set; }

        /// <summary>
        /// 对空（当前值，最大值）
        /// </summary>
        public Tuple<int, int> Taiku { get; internal set; }

        /// <summary>
        /// 雷装（当前值，最大值）
        /// </summary>
        public Tuple<int, int> Raisou { get; internal set; }

        /// <summary>
        /// 装甲（当前值，最大值）
        /// </summary>
        public Tuple<int, int> Soukou { get; internal set; }

        /// <summary>
        /// 回避（当前值，最大值）
        /// </summary>
        public Tuple<int, int> Kaihi { get; internal set; }

        /// <summary>
        /// 对潜（当前值，最大值）
        /// </summary>
        public Tuple<int, int> Taisen { get; internal set; }

        /// <summary>
        /// 索敌
        /// </summary>
        public Tuple<int, int> Sakuteki { get; set; }

        /// <summary>
        /// 运（当前值，最大值）
        /// </summary>
        public Tuple<int, int> Lucky { get; internal set; }

        /// <summary>
        /// 是否已锁
        /// </summary>
        public bool Locked { get; internal set; }

        /// <summary>
        /// 最大生命值
        /// </summary>
        public int MaxHP { get; internal set; }


        /// <summary>
        /// 当前生命值
        /// </summary>
        public int NowHP { get; internal set; }

        /// <summary>
        /// 入渠所需时间
        /// </summary>
        public TimeSpan DockTime { get; internal set; }

        /// <summary>
        /// 当前装备,No集合
        /// </summary>
        public int[] Slot { get; internal set; }

        /// <summary>
        /// 未知
        /// </summary>
        public int[] OnSlot { get; internal set; }

        internal KancolleShip(JToken api_ship_item)
        {
            No = api_ship_item["api_id"].ToObject<int>();
            ShipId = api_ship_item["api_ship_id"].ToObject<int>();
            Level = api_ship_item["api_lv"].ToObject<int>();
            Condition = api_ship_item["api_cond"].ToObject<int>();
            Karyoku = new Tuple<int, int>(((JArray)api_ship_item["api_karyoku"])[0].ToObject<int>(), ((JArray)api_ship_item["api_karyoku"])[1].ToObject<int>());
            Taiku = new Tuple<int, int>(((JArray)api_ship_item["api_taiku"])[0].ToObject<int>(), ((JArray)api_ship_item["api_taiku"])[1].ToObject<int>());
            Raisou = new Tuple<int, int>(((JArray)api_ship_item["api_raisou"])[0].ToObject<int>(), ((JArray)api_ship_item["api_raisou"])[1].ToObject<int>());
            Soukou = new Tuple<int, int>(((JArray)api_ship_item["api_soukou"])[0].ToObject<int>(), ((JArray)api_ship_item["api_soukou"])[1].ToObject<int>());
            Kaihi = new Tuple<int, int>(((JArray)api_ship_item["api_kaihi"])[0].ToObject<int>(), ((JArray)api_ship_item["api_kaihi"])[1].ToObject<int>());
            Taisen = new Tuple<int, int>(((JArray)api_ship_item["api_taisen"])[0].ToObject<int>(), ((JArray)api_ship_item["api_taisen"])[1].ToObject<int>());
            Sakuteki = new Tuple<int, int>(((JArray)api_ship_item["api_sakuteki"])[0].ToObject<int>(), ((JArray)api_ship_item["api_sakuteki"])[1].ToObject<int>());
            Lucky = new Tuple<int, int>(((JArray)api_ship_item["api_lucky"])[0].ToObject<int>(), ((JArray)api_ship_item["api_lucky"])[1].ToObject<int>());
            Locked = (api_ship_item["api_locked"].ToObject<int>()) == 0;
            MaxHP = api_ship_item["api_maxhp"].ToObject<int>();
            NowHP = api_ship_item["api_nowhp"].ToObject<int>();
            DockTime = TimeSpan.FromMilliseconds(api_ship_item["api_ndock_time"].ToObject<long>());
            Slot = api_ship_item["api_slot"].ToObject<int[]>();

            OnSlot = api_ship_item["api_onslot"].ToObject<int[]>();
        }


        /// <summary>
        /// 是否为大破状态
        /// </summary>
        /// <param name="NowHP">当前血量</param>
        /// <param name="MaxHP">最大血量</param>
        /// <returns></returns>
        public static bool IsBigBroken(int NowHP, int MaxHP)
        {
            return NowHP * 4 - 1 < MaxHP;
        }

        /// <summary>
        /// 是否为大破状态
        /// </summary>
        public bool BigBroken
        {
            get { return IsBigBroken(NowHP, MaxHP); }
        }

        /// <summary>
        /// 修理完成，血量恢复
        /// </summary>
        internal void RepairFinished()
        {
            NowHP = MaxHP;
        }

    }

    /// <summary>
    /// 舰娘信息（数据库）
    /// </summary>
    public class KancolleShipData
    {
        /// <summary>
        /// 数据库中的船的id
        /// </summary>
        public int ShipId { get; internal set; }

        /// <summary>
        /// 舰娘名
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// 船的类型
        /// </summary>
        public int Type { get; internal set; }

        /// <summary>
        /// 可装备的装备数
        /// </summary>
        public int SlotNum { get; internal set; }

        internal KancolleShipData(JToken api_mst_ship_item)
        {
            ShipId = api_mst_ship_item["api_id"].ToObject<int>();
            Name = api_mst_ship_item["api_name"].ToString();
            Type = api_mst_ship_item["api_stype"].ToObject<int>();
            SlotNum = api_mst_ship_item["api_slot_num"].ToObject<int>();
        }
    }

    /// <summary>
    /// 装备类型
    /// </summary>
    public class KancolleItemEquipType
    {
        /// <summary>
        /// 装备类型id
        /// </summary>
        public int TypeId { get; internal set; }

        /// <summary>
        /// 装备名
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// 不清楚
        /// </summary>
        public int ShowFlag { get; internal set; }

        internal KancolleItemEquipType(JToken api_mst_slotitem_equiptype_item)
        {
            TypeId = api_mst_slotitem_equiptype_item["api_id"].ToObject<int>();
            Name = api_mst_slotitem_equiptype_item["api_name"].ToString();
            ShowFlag = api_mst_slotitem_equiptype_item["api_show_flg"].ToObject<int>();
        }
    }

    /// <summary>
    /// 舰种
    /// </summary>
    public class KancolleShipType
    {
        /// <summary>
        /// 舰种id
        /// </summary>
        public int TypeId { get; internal set; }

        public int SortNo { get; internal set; }

        /// <summary>
        /// 舰种名
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// 入渠时间的倍率
        /// </summary>
        public int Scnt { get; internal set; }

        /// <summary>
        /// 建造时的剪影
        /// </summary>
        public int Kcnt { get; internal set; }

        /// <summary>
        /// 可装备的装备类型，key：ItemEquipType的id，value：0为不可用， 1为可用
        /// </summary>
        public ReadOnlyDictionary<int, int> EquipItemType { get; internal set; }

        internal KancolleShipType(JToken api_mst_stype_item)
        {
            TypeId = api_mst_stype_item["api_id"].ToObject<int>();
            SortNo = api_mst_stype_item["api_sortno"].ToObject<int>();
            Name = api_mst_stype_item["api_name"].ToString();
            Scnt = api_mst_stype_item["api_scnt"].ToObject<int>();
            Kcnt = api_mst_stype_item["api_kcnt"].ToObject<int>();
            EquipItemType = new ReadOnlyDictionary<int, int>(api_mst_stype_item["api_equip_type"].ToObject<Dictionary<int, int>>());
        }

    }

    /// <summary>
    /// 远征信息
    /// </summary>
    public class KancolleMissson
    {
        /// <summary>
        /// 远征数据库对应的id
        /// </summary>
        public int MissionId { get; internal set; }

        /// <summary>
        /// 远征状态
        /// </summary>
        public int State { get; internal set; }

        internal KancolleMissson(JToken api_mission_item)
        {
            MissionId = api_mission_item["api_mission_id"].ToObject<int>();
            State = api_mission_item["api_state"].ToObject<int>();
        }
    }


    /// <summary>
    /// 远征信息（数据库）
    /// </summary>
    public class KancolleMissionData
    {
        /// <summary>
        /// 远征Id
        /// </summary>
        public int MissionId { get; set; }

        /// <summary>
        /// 远征名
        /// </summary>
        public string Name { get; set; }

        internal KancolleMissionData(JToken api_mst_mission_item)
        {
            MissionId = api_mst_mission_item["api_id"].ToObject<int>();
            Name = api_mst_mission_item["api_name"].ToString();
        }
    }

    /// <summary>
    /// 资源信息
    /// </summary>
    public class KancolleMaterial
    {
        /// <summary>
        /// 燃
        /// </summary>
        public int Ran { get; internal set; }

        /// <summary>
        /// 弹
        /// </summary>
        public int Dan { get; internal set; }

        /// <summary>
        /// 钢
        /// </summary>
        public int Gang { get; internal set; }

        /// <summary>
        /// 铝
        /// </summary>
        public int Lv { get; internal set; }

        /// <summary>
        /// 建造资材
        /// </summary>
        public int Jianzao { get; internal set; }

        /// <summary>
        /// 修复桶
        /// </summary>
        public int Xiufu { get; internal set; }

        /// <summary>
        /// 开发资材
        /// </summary>
        public int Kaifa { get; internal set; }

        /// <summary>
        /// 改修紫菜
        /// </summary>
        public int Gaixiu { get; internal set; }

        internal KancolleMaterial() { }

        internal KancolleMaterial(JArray api_material_item)
        {
            if (api_material_item != null)
            {
                foreach (var material in api_material_item)
                {
                    switch (material["api_id"].ToObject<int>())
                    {
                        case 1:
                            Ran = material["api_value"].ToObject<int>();
                            break;
                        case 2:
                            Dan = material["api_value"].ToObject<int>();
                            break;
                        case 3:
                            Gang = material["api_value"].ToObject<int>();
                            break;
                        case 4:
                            Lv = material["api_value"].ToObject<int>();
                            break;
                        case 5:
                            Jianzao = material["api_value"].ToObject<int>();
                            break;
                        case 6:
                            Xiufu = material["api_value"].ToObject<int>();
                            break;
                        case 7:
                            Kaifa = material["api_value"].ToObject<int>();
                            break;
                        case 8:
                            Gaixiu = material["api_value"].ToObject<int>();
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 以materials为燃弹钢铝的值
        /// 剩余资材和桶以mat为准
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="materials"></param>
        public KancolleMaterial(KancolleMaterial mat, int[] materials)
        {
            Ran = materials[0];
            Dan = materials[1];
            Gang = materials[2];
            Lv = materials[3];

            Gaixiu = mat.Gaixiu;
            Kaifa = mat.Kaifa;
            Xiufu = mat.Xiufu;
            Jianzao = mat.Jianzao;
        }


    }

    /// <summary>
    /// 任务信息
    /// </summary>
    public class KancolleQuest
    {
        /// <summary>
        /// 任务Id
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// 任务种类
        /// </summary>
        public int Category { get; internal set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public int Type { get; internal set; }

        /// <summary>
        /// state=3表示完成，state=2表示任务接受 state=1表示未接收
        /// </summary>
        public int State { get; internal set; }

        /// <summary>
        /// 任务标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 任务细节
        /// </summary>
        public string Detail { get; internal set; }

        /// <summary>
        /// 资源报酬
        /// </summary>
        public Tuple<int, int, int, int> Material { get; internal set; }

        /// <summary>
        /// 0-0% 1-50% 2-80%
        /// </summary>
        public int ProgressFlag { get; internal set; }

        internal KancolleQuest(JToken api_questlist_item)
        {
            Id = api_questlist_item["api_no"].ToObject<int>();
            Category = api_questlist_item["api_category"].ToObject<int>();
            Type = api_questlist_item["api_type"].ToObject<int>();
            State = api_questlist_item["api_state"].ToObject<int>();
            Title = api_questlist_item["api_title"].ToString();
            Detail = api_questlist_item["api_detail"].ToString();
            var materials = api_questlist_item["api_get_material"].ToObject<int[]>();
            Material = new Tuple<int, int, int, int>(materials[0], materials[1], materials[2], materials[3]);
            ProgressFlag = api_questlist_item["api_progress_flag"].ToObject<int>();
        }
    }

    /// <summary>
    /// 地图信息（数据库）
    /// </summary>
    public class KancolleMapInfoData
    {
        /// <summary>
        /// 地图Id
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// 海域id
        /// </summary>
        public int MapAreaId { get; internal set; }

        /// <summary>
        /// 海域的第几个地图
        /// </summary>
        public int MapNo { get; internal set; }

        public string Name { get; internal set; }

        public int Level { get; internal set; }

        public string InfoText { get; internal set; }

        internal KancolleMapInfoData(JToken api_mst_mapinfo_item)
        {
            Id = api_mst_mapinfo_item["api_id"].ToObject<int>();
            MapAreaId = api_mst_mapinfo_item["api_maparea_id"].ToObject<int>();
            MapNo = api_mst_mapinfo_item["api_no"].ToObject<int>();
            Name = api_mst_mapinfo_item["api_name"].ToString();
            Level = api_mst_mapinfo_item["api_level"].ToObject<int>();
            InfoText = api_mst_mapinfo_item["api_infotext"].ToString();
        }
    }

    /// <summary>
    /// 装备信息
    /// </summary>
    public class KancolleSlotItem
    {
        /// <summary>
        /// 装备顺序编号
        /// </summary>
        public int No { get; internal set; }

        /// <summary>
        /// 对应到装备数据库的id
        /// </summary>
        public int SlotItemId { get; internal set; }

        public bool Locked { get; internal set; }

        public int Level { get; internal set; }

        internal KancolleSlotItem(JToken api_slot_item_item)
        {
            No = api_slot_item_item["api_id"].ToObject<int>();
            SlotItemId = api_slot_item_item["api_slotitem_id"].ToObject<int>();
            Locked = api_slot_item_item["api_locked"].ToObject<int>() != 0;
            Level = api_slot_item_item["api_level"].ToObject<int>();
        }
    }

    /// <summary>
    /// 装备信息（数据库）
    /// </summary>
    public class KancolleSlotItemData
    {
        public int Id { get; internal set; }

        /// <summary>
        /// 当前分类下顺序
        /// </summary>
        public int SortNo { get; internal set; }

        /// <summary>
        /// 装备名
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// 装备类型
        /// [0]:大分類
        /// [1]:夜戦判定
        /// [2]:装備可能艦種判定(即api_mst_slotitem_equiptype)
        /// [3]:不明
        /// </summary>
        public Tuple<int, int, int, int> Type { get; internal set; }

        /// <summary>
        /// 最大HP
        /// </summary>
        public int Taik { get; internal set; }

        /// <summary>
        /// 装甲
        /// </summary>
        public int Souk { get; internal set; }

        /// <summary>
        /// 火力
        /// </summary>
        public int Houg { get; set; }

        /// <summary>
        /// 雷装
        /// </summary>
        public int Raig { get; internal set; }

        /// <summary>
        /// 速度
        /// </summary>
        public int Soku { get; internal set; }

        /// <summary>
        /// Dive bomber??
        /// </summary>
        public int Baku { get; internal set; }

        /// <summary>
        /// 对空
        /// </summary>
        public int Tyku { get; internal set; }

        /// <summary>
        /// 对潜
        /// </summary>
        public int Tais { get; internal set; }

        /// <summary>
        /// Unused
        /// </summary>
        public int Atap { get; internal set; }

        /// <summary>
        /// 命中
        /// </summary>
        public int Houm { get; internal set; }

        /// <summary>
        /// Unkown
        /// </summary>
        public int Raim { get; internal set; }

        /// <summary>
        /// 回避
        /// </summary>
        public int Houk { get; internal set; }

        /// <summary>
        /// Unused
        /// </summary>
        public int Raik { get; internal set; }

        /// <summary>
        /// Unused
        /// </summary>
        public int Bakk { get; internal set; }

        /// <summary>
        /// Line of Sight??
        /// </summary>
        public int Saku { get; internal set; }

        /// <summary>
        /// Unused
        /// </summary>
        public int Sakb { get; internal set; }

        /// <summary>
        /// 运
        /// </summary>
        public int Luck { get; internal set; }

        /// <summary>
        /// 射程
        /// </summary>
        public int Leng { get; internal set; }

        /// <summary>
        /// 稀有度
        /// </summary>
        public int Rare { get; private set; }

        /// <summary>
        /// 废弃后所能获得的材料(燃弹钢铝)
        /// </summary>
        public Tuple<int, int, int, int> Broken { get; internal set; }

        /// <summary>
        /// 装备信息
        /// </summary>
        public string Info { get; internal set; }

        /// <summary>
        /// unused
        /// </summary>
        public string UseBull { get; internal set; }

        internal KancolleSlotItemData(JToken api_mst_slotitem_item)
        {
            Id = api_mst_slotitem_item["api_id"].ToObject<int>();
            SortNo = api_mst_slotitem_item["api_sortno"].ToObject<int>();
            Name = api_mst_slotitem_item["api_name"].ToString();
            var types = api_mst_slotitem_item["api_type"].ToObject<int[]>();
            Type = new Tuple<int, int, int, int>(types[0], types[1], types[2], types[3]);
            Taik = api_mst_slotitem_item["api_taik"].ToObject<int>();
            Souk = api_mst_slotitem_item["api_souk"].ToObject<int>();
            Houg = api_mst_slotitem_item["api_houg"].ToObject<int>();
            Raig = api_mst_slotitem_item["api_raig"].ToObject<int>();
            Soku = api_mst_slotitem_item["api_soku"].ToObject<int>();
            Baku = api_mst_slotitem_item["api_baku"].ToObject<int>();
            Tyku = api_mst_slotitem_item["api_tyku"].ToObject<int>();
            Tais = api_mst_slotitem_item["api_tais"].ToObject<int>();
            Atap = api_mst_slotitem_item["api_atap"].ToObject<int>();
            Houm = api_mst_slotitem_item["api_houm"].ToObject<int>();
            Raim = api_mst_slotitem_item["api_raim"].ToObject<int>();
            Houk = api_mst_slotitem_item["api_houk"].ToObject<int>();
            Raik = api_mst_slotitem_item["api_raik"].ToObject<int>();
            Bakk = api_mst_slotitem_item["api_bakk"].ToObject<int>();
            Saku = api_mst_slotitem_item["api_saku"].ToObject<int>();
            Sakb = api_mst_slotitem_item["api_sakb"].ToObject<int>();
            Luck = api_mst_slotitem_item["api_luck"].ToObject<int>();
            Rare = api_mst_slotitem_item["api_rare"].ToObject<int>();
            var brokens = api_mst_slotitem_item["api_broken"].ToObject<int[]>();
            Broken = new Tuple<int, int, int, int>(brokens[0], brokens[1], brokens[2], brokens[3]);
            Info = api_mst_slotitem_item["api_info"].ToString();
            UseBull = api_mst_slotitem_item["api_usebull"].ToString();
        }
    }

    /// <summary>
    /// 入渠信息
    /// </summary>
    public class KancolleDockData
    {
        /// <summary>
        /// 入渠的舰娘ownedNo
        /// </summary>
        public int ShipId { get; internal set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime CompleteTime { get; internal set; }

        /// <summary>
        /// -1:未拥有；0：已拥有，且空着；1：已拥有，但是被占用
        /// </summary>
        public int State { get; internal set; }

        internal KancolleDockData(JToken api_ndock_item)
        {
            ShipId = api_ndock_item["api_ship_id"].ToObject<int>();
            CompleteTime = ParseLongTime(api_ndock_item["api_complete_time"].ToObject<long>());
            State = api_ndock_item["api_state"].ToObject<int>();
        }


        public static DateTime ParseLongTime(long time)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(time + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow);
            return dtResult;
        }

    }
}
