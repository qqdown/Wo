using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Wo.Utils;

namespace Wo.KancolleCore
{
    class KancolleGameContext
    {
        public event Action<KancolleGameContext> OnGameDataUpdated;

        private KancolleGameData gameData = new KancolleGameData();

        public void OnAPIResponseReceivedHandler(RequestInfo requestInfo, string response, string api)
        {
            var data = JsonConvert.DeserializeObject(response) as JObject;
            var api_object = data["api_data"];
            bool gameDataUpdated = true;

            switch (api)
            {
                case "api_start2":
                    UpdateShipDatas(api_object["api_mst_ship"] as JArray);
                    UpdateMissions(api_object["api_mst_mission"] as JArray);
                    UpdateMissions(api_object["api_mst_mission"] as JArray);
                    UpdateMapInfoDictionary(api_object["api_mst_mapinfo"] as JArray);
                    UpdateSlotDictionary(api_object["api_mst_slotitem"] as JArray);
                    UpdateShipTypeDictionary(api_object["api_mst_stype"] as JArray);
                    UpdateItemEquipTypeDictionary(api_object["api_mst_slotitem_equiptype"] as JArray);
                    break;
                case "api_port/port":
                    UpdateOwnedShips(api_object["api_ship"] as JArray);
                    UpdateMaterial(api_object["api_material"] as JArray);
                    UpdatePort(api_object);
                    UpdateDeck(api_object["api_deck_port"] as JArray);
                    UpdateDock(api_object["api_ndock"] as JArray);
                    break;
                case "api_req_hensei/change"://舰队编成修改
                    UpdateDeck(requestInfo.Data);
                    break;
                case "api_get_member/material":
                    UpdateMaterial(api_object as JArray);
                    break;
                case "api_req_hokyu/charge":
                    UpdateMaterial(api_object["api_material"].ToObject<int[]>());
                    break;
                case "api_get_member/require_info":
                    UpdateOwnedSlotDictionary(api_object["api_slot_item"] as JArray);
                    break;
                case "api_get_member/slot_item":
                    UpdateOwnedSlotDictionary(api_object as JArray);
                    break;
                case "api_get_member/questlist":
                    UpdateQuest(api_object);
                    break;
                case "api_get_member/mission"://可进行的远征任务
                    UpdateOwnedMissionDictionary(api_object as JArray);
                    break;
                case "api_get_member/ship3":
                    UpdateShip3(api_object);
                    break;
                case "api_req_kaisou/slot_deprive"://更换在已有舰娘身上的装备
                    UpdateDeprived(api_object);
                    break;
                case "api_get_member/ndock"://获取入渠信息
                    UpdateDock(api_object as JArray);
                    break;
                case "api_get_member/ship_deck"://战斗后的舰娘信息
                    UpdateShips(api_object["api_ship_data"] as JArray);
                    break;
                case "api_req_sortie/battle":
                    break;
                default:
                    gameDataUpdated = false;
                    break;
            }
            if (gameDataUpdated)
            {
                //更新装备情况
                UpadteOwnedSlotItemEquipInfo();

                try
                {
                    OnGameDataUpdated?.InvokeAll(this);
                }
                catch (AggregateException exList)
                {
                    foreach (var ex in exList.InnerExceptions)
                    {
                        MessageBox.Show("OnGameDataUpdated出错！\n" + ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }
        }

        #region private methods
        /// <summary>
        /// 更新从api_mst_ship中获得的舰娘数据信息
        /// </summary>
        /// <param name="items"></param>
        private void UpdateShipDatas(JArray items)
        {
            gameData.ShipDataDictionary = new ReadOnlyDictionary<int, KancolleShipData>(items.ToDictionary(
                k => k["api_id"].ToObject<int>(),
                k => new KancolleShipData(k)
                ));
        }

        //api_mst_slotitem_equiptype_item
        private void UpdateItemEquipTypeDictionary(JArray items)
        {
            gameData.ItemEquipTypeDictionary = new ReadOnlyDictionary<int, KancolleItemEquipType>(items.ToDictionary(
                k => k["api_id"].ToObject<int>(),
                k => new KancolleItemEquipType(k)));
        }

        //api_mst_stype_item
        private void UpdateShipTypeDictionary(JArray items)
        {
            gameData.ShipTypeDictionary = new ReadOnlyDictionary<int, KancolleShipType>(items.ToDictionary(
                k => k["api_id"].ToObject<int>(),
                k => new KancolleShipType(k)));
        }

        /// <summary>
        /// 更新从api_mst_mapinfo_item获取的地图信息
        /// </summary>
        /// <param name="api_mst_mapinfo"></param>
        private void UpdateMapInfoDictionary(JArray api_mst_mapinfo)
        {
            gameData.MapInfoDictionary = new ReadOnlyDictionary<int, KancolleMapInfoData>(api_mst_mapinfo.ToDictionary(
                k => k["api_id"].ToObject<int>(),
                k => new KancolleMapInfoData(k)));
        }

        /// <summary>
        /// 更新从api_mst_slotitem_item中获得的装备数据信息
        /// </summary>
        /// <param name="api_mst_slotitem"></param>
        private void UpdateSlotDictionary(JArray api_mst_slotitem)
        {
            gameData.SlotDictionary = new ReadOnlyDictionary<int, KancolleSlotItemData>(api_mst_slotitem.ToDictionary(
                k => k["api_id"].ToObject<int>(),
                k => new KancolleSlotItemData(k)));
        }


        /// <summary>
        /// 更新从api_slot_item_item中获得到的装备信息
        /// </summary>
        /// <param name="api_slot_item"></param>
        private void UpdateOwnedSlotDictionary(JArray api_slot_item)
        {
            gameData.OwnedSlotDictionary = new ReadOnlyDictionary<int, KancolleSlotItem>(api_slot_item.ToDictionary(
                k => k["api_id"].ToObject<int>(),
                k => new KancolleSlotItem(k)));
        }

        /// <summary>
        /// 更新从api_ship中获得的当前已有的舰娘信息
        /// </summary>
        /// <param name="items"></param>
        private void UpdateOwnedShips(JArray items)
        {
            //if (api_mst_ship == null)
            //    throw new Exception("需要先更新舰娘数据信息api_mst_item");
            gameData.OwnedShipDictionary = new ReadOnlyDictionary<int, KancolleShip>(items.ToDictionary(
                    k => k["api_id"].ToObject<int>(),
                    k => new KancolleShip(k)
                    ));
        }

        //api_mission_item
        private void UpdateOwnedMissionDictionary(JArray ownedMissionItems)
        {
            gameData.OwnedMissionDic = new ReadOnlyDictionary<int, KancolleMissson>(ownedMissionItems.ToDictionary(
                k => k["api_id"].ToObject<int>(),
                k => new KancolleMissson(k)));
        }

        /// <summary>
        /// 更新从api_mst_mission中获得的远征数据信息
        /// </summary>
        /// <param name="missions"></param>
        private void UpdateMissions(JArray missions)
        {
            gameData.MissionDictionary = new ReadOnlyDictionary<int, KancolleMissionData>(missions.ToDictionary(
                k => k["api_id"].ToObject<int>(),
                k => new KancolleMissionData(k)
                ));
        }

        /// <summary>
        /// 更新提督基本信息
        /// </summary>
        /// <param name="portData"></param>
        private void UpdatePort(JToken portData)
        {
            if (portData != null)
            {
                gameData.BasicInfo = new KancolleBasicInfo(portData);
            }
        }

        /// <summary>
        /// 调用port的API时会更新deck
        /// </summary>
        /// <param name="deckPorts"></param>
        private void UpdateDeck(JArray deckPorts)
        {
            var dic = new Dictionary<int, Tuple<int, int>>();
            var array = gameData.OwnedShipPlaceArray.ToArray();
            for (int i = 0; i < deckPorts.Count; i++)
            {
                var api_ships = deckPorts[i]["api_ship"] as JArray;
                for (int j = 0; j < api_ships.Count; j++)
                {
                    int ownedShipId = api_ships[j].ToObject<int>();//在第i个舰队第j个位置的舰娘id
                    if (ownedShipId > 0)
                    {
                        var ship = gameData.OwnedShipDictionary[ownedShipId];
                        dic.Add(ownedShipId, new Tuple<int, int>(i, j));
                    }
                    array[i, j] = ownedShipId;

                }
            }
            gameData.OwnedShipPlaceDictionary = new ReadOnlyDictionary<int, Tuple<int, int>>(dic);
            gameData.OwnedShipPlaceArray = new Utils.ReadOnlyArray2<int>(array);
        }

        /// <summary>
        /// 当改变船的编成位置时更新deck;api_req_hensei/change时调用
        /// </summary>
        /// <param name="shipChangePostData"></param>
        private void UpdateDeck(Dictionary<string, string> shipChangePostData)
        {
            int api_id = int.Parse(shipChangePostData["api_id"]);
            int deck_index = api_id - 1;
            int api_ship_id = int.Parse(shipChangePostData["api_ship_id"]);
            int api_ship_idx = int.Parse(shipChangePostData["api_ship_idx"]);
            var array = gameData.OwnedShipPlaceArray.ToArray();
            if (api_ship_id > 0)//添加
            {
                var ship = gameData.OwnedShipDictionary[api_ship_id];

                //交换自己的位置和目标位置
                if (gameData.OwnedShipPlaceDictionary.ContainsKey(api_ship_id))//本来就在编队中
                {
                    var place = gameData.OwnedShipPlaceDictionary[api_ship_id];//本来的位置
                    int dstShip = gameData.OwnedShipPlaceArray[deck_index, api_ship_idx];//目标位置的船
                    if (dstShip == -1) //目标位置为空
                    {
                        //直接添加
                        array[deck_index, api_ship_idx] = api_ship_id;
                        //原位置清空      
                        array[place.Item1, place.Item2] = -1;
                    }
                    else//目标位置有舰娘
                    {
                        //交换
                        array[deck_index, api_ship_idx] = api_ship_id;
                        array[place.Item1, place.Item2] = dstShip;
                    }
                }
                else//本来不在编队中
                {
                    int dstShip = gameData.OwnedShipPlaceArray[deck_index, api_ship_idx];
                    if (dstShip == -1) //目标位置为空
                    {
                        //直接添加
                        array[deck_index, api_ship_idx] = api_ship_id;
                    }
                    else//目标位置有舰娘
                    {
                        //交换
                        array[deck_index, api_ship_idx] = api_ship_id;
                    }
                }
            }
            else if (api_ship_id == -1)//移除
            {
                array[deck_index, api_ship_idx] = -1;

            }
            else if (api_ship_id == -2)//移除旗舰外所有舰船
            {
                for (int i = 1; i < array.GetLength(1); i++)
                {
                    array[deck_index, i] = -1;
                }
            }

            //所有中间为空的船位都需要向前合并
            for (int i = 0; i < array.GetLength(0); i++)
            {
                int realPlace = 0;
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    if (array[i, j] > 0)
                        array[i, realPlace++] = array[i, j];
                }
                for (; realPlace < array.GetLength(1); realPlace++)
                {
                    array[i, realPlace] = -1;
                }
            }

            //生成位置检索字典
            Dictionary<int, Tuple<int, int>> dic = new Dictionary<int, Tuple<int, int>>();
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    if (array[i, j] > 0)
                        dic.Add(array[i, j], new Tuple<int, int>(i, j));
                }
            }
            gameData.OwnedShipPlaceDictionary = new ReadOnlyDictionary<int, Tuple<int, int>>(dic);
            gameData.OwnedShipPlaceArray = new Utils.ReadOnlyArray2<int>(array);
        }

        /// <summary>
        /// 更新资源信息
        /// </summary>
        /// <param name="materials"></param>
        private void UpdateMaterial(JArray api_material_items)
        {
            if (api_material_items != null)
            {
                gameData.Material = new KancolleMaterial(api_material_items);
            }

        }

        /// <summary>
        /// 更新资源信息，只更新燃弹钢铝
        /// </summary>
        /// <param name="materials"></param>
        private void UpdateMaterial(int[] materials)
        {
            if (materials != null)
            {
                gameData.Material = new KancolleMaterial(gameData.Material, materials);
            }
        }

        //api_ndock_item
        private void UpdateDock(JArray api_docks)
        {
            KancolleDockData[] docks = new KancolleDockData[api_docks.Count];
            for (int i = 0; i < api_docks.Count; i++)
            {
                if (i < gameData.DockArray.Count && gameData.DockArray[i]?.State > 0 && api_docks[i]["api_state"].ToObject<int>() == 0)
                {
                    KancolleGameData.Instance.OwnedShipDictionary[gameData.DockArray[i].ShipId].RepairFinished();
                }
                docks[i] = new KancolleDockData(api_docks[i]);
            }
            gameData.DockArray = new ReadOnlyCollection<KancolleDockData>(docks);
        }

        /// <summary>
        /// 更新任务信息
        /// api_questlist_data
        /// </summary>
        /// <param name="questData"></param>
        private void UpdateQuest(JToken questData)
        {
            if (questData != null && questData["api_list"] != null)
            {
                JArray validQuestList = questData["api_list"] as JArray;

                if (validQuestList.Count > 0)
                {
                    Dictionary<int, KancolleQuest> dic = gameData.QuestDictionary.ToDictionary(k => k.Key, k => k.Value);
                    foreach (var quest in validQuestList)
                    {
                        int api_no = quest["api_no"].ToObject<int>();
                        if (dic.ContainsKey(api_no))
                            dic[api_no] = new KancolleQuest(quest);
                        else
                            dic.Add(api_no, new KancolleQuest(quest));
                    }
                    gameData.QuestDictionary = new ReadOnlyDictionary<int, KancolleQuest>(dic);
                }
            }
        }

        /// <summary>
        /// 更新装备是否倍装备的情况，需要放在api处理switch之后
        /// </summary>
        private void UpadteOwnedSlotItemEquipInfo()
        {
            Dictionary<int, int> equipDic = new Dictionary<int, int>();
            List<int> unEquipArray = new List<int>();
            foreach (var ship in gameData.OwnedShipDictionary.Values)
            {
                foreach (var slot in ship.Slot)
                {
                    if (slot > 0)
                    {
                        equipDic.Add(slot, ship.No);
                    }
                }
            }

            foreach (var slot in gameData.OwnedSlotDictionary.Keys)
            {
                if (!equipDic.ContainsKey(slot))
                    unEquipArray.Add(slot);
            }

            gameData.EquipedSlotDictionary = new ReadOnlyDictionary<int, int>(equipDic);
            gameData.UnEquipedSlotArray = new ReadOnlyCollection<int>(unEquipArray);
        }


        //api_ship3_data
        private void UpdateShip3(JToken ship3)
        {
            var api_ship_data = ship3["api_ship_data"] as JArray;
            if (api_ship_data?.Count > 0)
            {
                var dic = gameData.OwnedShipDictionary.ToDictionary(k => k.Key, k => k.Value);
                foreach (var ship in api_ship_data)
                {
                    dic[ship["api_id"].ToObject<int>()] = new KancolleShip(ship);
                }
                gameData.OwnedShipDictionary = new ReadOnlyDictionary<int, KancolleShip>(dic);
            }

            //TODO
            //if (ship3.api_deck_data?.Length > 0)
            //{
              
            //}

            //if(ship3.api_slot_data != null)
            //{
            //    var dic = gameData.OwnedSlotDictionary.ToDictionary(k => k.Key, k => k.Value);

            //}
        }

        //api_slot_deprive_data
        private void UpdateDeprived(JToken deprive)
        {
            var dic = gameData.OwnedShipDictionary.ToDictionary(k => k.Key, k => k.Value);
            dic[deprive["api_ship_data"]["api_set_ship"]["api_id"].ToObject<int>()] = new KancolleShip(deprive["api_ship_data"]["api_set_ship"]);
            dic[deprive["api_ship_data"]["api_unset_ship"]["api_id"].ToObject<int>()] = new KancolleShip(deprive["api_ship_data"]["api_unset_ship"]);
            gameData.OwnedShipDictionary = new ReadOnlyDictionary<int, KancolleShip>(dic);
        }


        /// <summary>
        /// 仅更新传入参数部分的舰娘信息
        /// api_ship_item
        /// </summary>
        /// <param name="ship_items"></param>
        private void UpdateShips(JArray ship_items)
        {
            var dic = gameData.OwnedShipDictionary.ToDictionary(k => k.Key, k => k.Value);
            for (int i = 0; i < ship_items.Count; i++)
            {
                dic[ship_items[i]["api_id"].ToObject<int>()] = new KancolleShip(ship_items[i]);
            }
            gameData.OwnedShipDictionary = new ReadOnlyDictionary<int, KancolleShip>(dic);
        }
        #endregion

    }
}
