using System.ComponentModel;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using static RealRealTextRPG.Program;
using static RealRealTextRPG.Program.ItemManager;
using static System.Formats.Asn1.AsnWriter;

namespace RealRealTextRPG
{
    internal class Program
    {
        public delegate void EquipHandler();

        //클래스 분리
        //sceme매니저 생성자에서

        public class SeceneManager
        {
            private Player player;
            private IItems items;
            private Queue<Dungeon> dungeons;


            public SeceneManager(Player player, ItemManager itemManager)
            {
                this.player = player;
                player.itemManager = itemManager;     //이부분 player 에서도 만듬
                dungeons = new Queue<Dungeon>();
            }



            public void AddDungeon(Dungeon dungeon)
            {
                dungeons.Enqueue(dungeon);
            }

            public void GameMenu(Player player)
            {
                bool chooseMenu = false;

                Console.Clear();
                Console.WriteLine("스파르타 마을에 오신 것을 환영합니다.");
                Console.WriteLine("당신의 행동을 정해주세요");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("\t 1. 상태 보기");
                Console.WriteLine();
                Console.WriteLine("\t 2. 인벤토리");
                Console.WriteLine();
                Console.WriteLine("\t 3. 장비창");
                Console.WriteLine();
                Console.WriteLine("\t 4. 상점");
                Console.WriteLine();
                Console.WriteLine("\t 5. 휴식하기");
                Console.WriteLine();
                Console.WriteLine("\t 6. 던전입장");
                Console.WriteLine();
                Console.WriteLine();

                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                    case "상태 보기":
                    case "상태보기":
                        {
                            Console.Clear();
                            chooseMenu = true;
                            StatusCheck(player);
                            break;
                        }
                    case "2":
                    case "인벤토리":
                        {
                            Console.Clear();
                            chooseMenu = true;
                            player.itemManager.ShowInvertory(player);
                            break;
                        }
                    case "3":
                    case "장비창":
                        {
                            Console.Clear();
                            chooseMenu = true;
                            Equippage(player);
                            break;
                        }
                    case "4":
                    case "상점":
                        {
                            Console.Clear();
                            chooseMenu = true;
                            OpenStore(player);
                            break;
                        }
                    case "5":
                    case "휴식하기":
                        {
                            Console.Clear();
                            chooseMenu = true;
                            player.Rest();
                            break;
                        }
                    case "6":
                    case "던전입장":
                    case "던전 입장":
                        {
                            Console.Clear();
                            chooseMenu = true;
                            EnterDungeon();
                            break;
                        }
                    default:
                        Console.WriteLine("잘못된 입력입니다.");
                        GameMenu(player);
                        break;



                }


            }

            public void Equippage(Player player)
            {
                player.itemManager.Equippage(player);
                GameMenu(player);
            }

            public void ShowInvertory(Player player)
            {
                player.itemManager.ShowInvertory(player);
                GameMenu(player);
            }

            public void OpenStore(Player player)
            {
                player.itemManager.OpenStore(player);
                GameMenu(player);

            }

            public void Rest()
            {
                player.Rest();
                GameMenu(player);

            }

            public void StatusCheck(Player player)
            {
                player.StatusCheck(player);
                GameMenu(player);

            }

            public void EnterDungeon()
            {
                if (dungeons.Count > 0)
                {
                    Dungeon currentDungeon = dungeons.Dequeue();
                    currentDungeon.DungeonStart();
                }
                else
                {
                    Console.WriteLine("모든 던전을 클리어하셨습니다.");
                }
            }

        }

        public interface ICharacter
        {
            string Name { get; }
            int Health { get; set; }
            int Attack { get; }
            bool IsDead { get; }
            int currentHP { get; set; }
            void TakeDamage(int damage);
            void GoldChange(int amount);
            void GainExp(int amount);

        }

        public class Player : ICharacter
        {
            public string Name { get; set; }
            public int Health { get; set; }
            public int AttackPower { get; set; }
            public string Job { get; set; }
            public int currentHP { get; set; }

            public bool IsDead => currentHP <= 0;
            public int Attack => new Random().Next(AttackPower / 2, AttackPower);
            public int MaxHP => Health * 2;

            public ItemManager itemManager;



            public int gold = 0;

            private int exp = 0;
            private int maxExp = 100;
            public int level = 1;

            public Player(string name)
            {
                Name = name;
                Health = 50;
                currentHP = 100;
                AttackPower = 20;
                Job = "전사";
            }



            public void TakeDamage(int damage)
            {
                currentHP -= damage;
                if (IsDead == true)
                {
                    Console.WriteLine($"{Name}(이)가 죽었습니다.");
                }
                else
                {
                    Console.WriteLine($"{Name}(이)가 {damage}의 데미지를 입었습니다. \n남은 체력 : {currentHP}");
                }
            }
            public void HPChange(int amount)
            {
                currentHP += amount;
                if (currentHP > MaxHP)
                {
                    currentHP = MaxHP;
                }
                else
                {
                    return;
                }
            }
            public void HealthChange(int amount)
            {
                Health += amount;
                currentHP = MaxHP;
            }

            //레벨에 따라 얻어야 하는 최대 경험치를 증가시킴
            public void GainExp(int amount)
            {
                exp += amount;
                if (exp >= maxExp)
                {
                    exp -= maxExp;
                    LevelUp();
                }
            }
            private void LevelUp()
            {
                level++;
                if (level < 10)
                {
                    maxExp = (int)(maxExp * 1.1);
                    Health += 3;
                }
                else if (level < 20)
                {
                    maxExp = (int)(maxExp * 1.3);
                    Health += 4;
                }
                else
                {
                    maxExp = (int)(maxExp * 1.5);
                    Health += 5;
                }
                HealthChange(10);
            }
            public void GoldChange(int amount)
            {
                gold += amount;

                if (amount > 0)
                {
                    Console.WriteLine($"{amount}의 골드를 획득하였습니다. \n남은골드 : {gold}");
                }
                else if (amount < 0)
                {
                    Console.WriteLine($"{amount}의 골드를 사용하였습니다. \n남은골드 : {gold}");
                }
            }
            public void GetGold(int amount)
            {
                if (amount > 0)
                {
                    gold += amount;
                    Console.WriteLine($"{amount}의 골드를 획득하였습니다. \n남은골드 : {gold}");
                }
                else if (amount < 0)
                {
                    Console.WriteLine($"{amount}의 골드를 사용하였습니다. \n남은골드 : {gold}");
                }

            }

            public void StatusCheck(Player player)
            {
                Console.WriteLine("당신의 현재 상태입니다");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine($"레벨 :{player.level}\n직업 : {player.Job}\n공격력 : {player.Attack} + ({itemManager.attackWorth})");
                Console.WriteLine($"최대 체력 : {player.MaxHP}({itemManager.HealthWorth})\t현재 체력 : {player.currentHP}\n보유 금액 : {player.gold}");
                Console.WriteLine();
                Console.Write("아무키나 누르면 게임메뉴로 돌아갑니다.");
                Console.ReadLine();
            }

            public void Rest()
            {
                //골드가 500 이상일 때 모든 체력을 회복하고
                if (gold >= 500)
                {
                    gold -= 500;
                    Console.Clear();
                    Console.WriteLine("당신은 여관에서 잠에 들었습니다.");
                    Thread.Sleep(500);
                    Console.Write(".");
                    Thread.Sleep(500);
                    Console.Write(".");
                    Thread.Sleep(500);
                    Console.WriteLine(".");
                    Thread.Sleep(1500);
                    Console.WriteLine("아침이 밝았습니다.!");
                    currentHP = MaxHP;
                }
                else if (gold < 500)
                {
                    Console.WriteLine("골드가 부족합니다.");
                    Console.WriteLine("50의 체력만을 회복합니다.");
                    HPChange(50);
                    Thread.Sleep(1500);

                }
            }

        }

        public class ItemManager
        {
            public int attackWorth = 0;
            public int HealthWorth = 0;

            //인벤토리
            public Dictionary<Items, int> inventory = new Dictionary<Items, int>()
            {
                {Items.수련자_갑옷, 0 },
                {Items.무쇠갑옷, 0 },
                {Items.스파르타의_갑옷, 0 },
                {Items.낡은_검, 0 },
                {Items.청동_도끼, 0 },
                {Items.스파르타의_창, 0 },
                {Items.체력포션, 0 },
                {Items.힘포션, 0 }

            };
            public class Product
            {
                public Items Item { get; set; }
                public int Price { get; set; }
                public int Worth { get; set; }
                public bool IsSold { get; set; }
            }

            //상점아이템 + 가격
            public List<Product> store = new List<Product>
            {
                new Product {Item = Items.수련자_갑옷, Price = 1000, IsSold = false, Worth = 5},
                new Product {Item = Items.무쇠갑옷, Price = 2000, IsSold = false,Worth = 9},
                new Product {Item = Items.스파르타의_갑옷, Price = 3500, IsSold = false,Worth = 15},
                new Product {Item = Items.낡은_검, Price = 1000, IsSold = false,Worth = 5},
                new Product {Item = Items.청동_도끼, Price = 1000, IsSold = false, Worth = 9},
                new Product {Item = Items.스파르타의_창, Price = 1000, IsSold = false, Worth = 15},
                new Product {Item = Items.체력포션, Price = 1000, IsSold = false, Worth = 50},
                new Product {Item = Items.힘포션, Price = 1000, IsSold = false, Worth = 50}
            };
            public bool[] isHaveItem = new bool[8];
            public bool[] isEquipItem = new bool[6];


            public void ItemCountChange(Items item, int amount)
            {
                //아이템 갯수 변화 
                inventory[item] += amount;
                //아이템의 갯수 변화
                if (inventory[item] <= 0)
                {
                    inventory[item] = 0;
                    Console.WriteLine("이 아이템은 인벤토리에 없습니다.");
                    isHaveItem[(int)item] = false;
                }
                else if (inventory[item] > 0)
                {
                    isHaveItem[(int)item] = true;
                }
            }

            public string itemMessage(Items item)
            {
                switch (item)
                {
                    case Items.수련자_갑옷:
                        return "방어력 + 5      |수련에 도움을 주는 갑옷입니다.                     |";
                    case Items.무쇠갑옷:
                        return "방어력 + 9      |무쇠로 만들어져 튼튼한 갑옷입니다.                 |";
                    case Items.스파르타의_갑옷:
                        return "방어력 + 15     | 스파르타의 전사들이 사용했다는 전설의 갑옷입니다. |";
                    case Items.낡은_검:
                        return "공격력 + 5      |쉽게 볼 수 있는 낡은 검 입니다.                    |";
                    case Items.청동_도끼:
                        return "공격력 + 9      |어디선가 사용됐던거 같은 도끼입니다.               |";
                    case Items.스파르타의_창:
                        return "공격력 + 15     |스파르타의 전사들이 사용했다는 전설의 창입니다.    |";
                    case Items.체력포션:
                        return "체력 50 증가    |마시면 몸에 활기가 돋는 포션입니다.                |";
                    case Items.힘포션:
                        return "공격력 10 증가  |마시면 알수 없는 힘이 생겨납니다.                  |";
                    default:
                        return "잘못 입력되었습니다.";

                }
            }
            public void Sell(Items item, int amount, Player player)
            {
                int value = inventory[item];
                var product = store.FirstOrDefault(p => p.Item == item);
                if (product != null && value >= amount)
                {
                    value -= amount;
                    player.GoldChange(amount * product.Price);
                    inventory[item] = value;
                    if(inventory[item] <= 0)
                    {
                        isHaveItem[(int)item] = false;
                    }

                }
                else if (product == null)
                {
                    Console.WriteLine("아이템을 찾을 수 없습니다.");
                }
                else
                {
                    Console.WriteLine("아이템 수량이 부족합니다.");

                }
            }

            public void Buy(Items item, int amount, Player player)
            {
                int value = inventory[item];
                var product = store.FirstOrDefault(p => p.Item == item);

                if (product != null && product.IsSold == false)
                {
                    //골드가 부족한지 충분한지 확인 후
                    if (product != null && player.gold >= product.Price * amount)
                    {
                        //충분하다면 price * amount의 골드를 지불하고
                        player.GoldChange((product.Price * amount) * -1);

                        //인벤토리에 1을 추가한 뒤
                        value += amount;
                        inventory[item] = value;

                        //012, 345의 경우는 isSold를 뒤집어준다.
                        if ((int)item >= 0 && (int)item < 6)
                        {
                            product.IsSold = !product.IsSold;
                        }

                        if (inventory[item] <= 0)
                        {
                            isHaveItem[(int)item] = false;
                        }
                        else if( inventory[item] > 0)
                        {
                            isHaveItem[(int)item] = true;
                        }


                    }
                    else if (product == null)
                    {
                        Console.WriteLine("아이템을 찾을 수 없습니다.");
                    }
                    else
                    {
                        Console.WriteLine("골드가 부족합니다.");
                    }

                }
                else if (product != null && product.IsSold == false)
                {
                    Console.WriteLine("구매 완료된 물품입니다.");
                }
                else
                {
                    Console.WriteLine("아이템을 찾을 수 없습니다.");
                    string input = Console.ReadLine();
                }
            }

            public void PotionPage(Player player)
            {
                IItems items;

                while (true)
                {
                    Console.WriteLine();
                    Console.WriteLine("[보유중인 포션]");
                    Console.WriteLine();
                    Console.WriteLine($"1. {Items.체력포션}이 {inventory[Items.체력포션]}개 있습니다.");
                    Console.WriteLine($"2. {Items.힘포션}이 {inventory[Items.힘포션]}개 있습니다.");
                    Console.WriteLine("사용할 포션의 종류를 말해주세요.");
                    Console.WriteLine("나가고싶으면 0번을 눌러주세요");
                    string input = Console.ReadLine();

                    if (input == "1" || input == "힘 포션" || input == "힘포션")
                    {
                        items = new StrengthPotion();
                        items.Use(player, Items.힘포션, 1);
                        Console.WriteLine("힘포션 을 사용하여 공격력 10 증가합니다.");
                        break;
                    }
                    else if (input == "2" || input == "체력 포션" || input == "체력포션")
                    {
                        items = new HealthPotion();
                        items.Use(player, Items.체력포션, 1);
                        Console.WriteLine("체력포션을 사용하여 HP가 50 증가합니다.");
                        break;
                    }
                }

            }

            public void EquipItem(Items item, Player player)
            {
                //해당 아이템이 인벤토리에 있어야하며, 그 갯수가 0이상일 때
                if (inventory.TryGetValue(item, out int count) && count > 0)
                {
                    //아이템 종류별로 하나씩만 장착가능
                    if ((int)item >= 0 && (int)item < 3)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (isEquipItem[i] == true)
                            {
                                isEquipItem[i] = false;
                                player.HealthChange(store[i].Worth * -2);
                                HealthWorth -= store[i].Worth * 2;
                            }
                        }

                        isEquipItem[(int)item] = true;
                        player.HealthChange(store[(int)item].Worth * 2);
                        HealthWorth += store[(int)item].Worth * 2;

                        Console.WriteLine($"아이템을 장착하여 건강이 {store[(int)item].Worth} 증가하였습니다.");
                        
                    }
                    else if ((int)item >= 3 && (int)item < 6)
                    {
                        for (int i = 3; i < 6; i++)
                        {
                            if (isEquipItem[i] == true)
                            {
                                isEquipItem[i] = false;
                                player.AttackPower -= store[i].Worth * 3;
                                attackWorth -= store[i].Worth * 3;
                            }
                        }

                        isEquipItem[(int)item] = true;
                        player.AttackPower += store[(int)item].Worth * 3;
                        attackWorth += store[(int)item].Worth * 3;
                        Console.WriteLine($"아이템을 장착하여 공격력이 {store[(int)item].Worth} 증가하였습니다.");

                    }
                }
                Console.WriteLine("해당 아이템의 수량이 부족합니다.");
            }

            public void Equippage(Player player)
            {


                Console.WriteLine("보유 중인 아이템을 관리 할 수 있습니다.");
                Console.WriteLine();
                Console.WriteLine("[보유 중인 아이템 목록]");

                foreach (KeyValuePair<Items, int> i in inventory)
                {
                    if (isHaveItem[(int)i.Key] && (int)i.Key < 6)
                    {
                        string status = isEquipItem[(int)i.Key] ? "[E]" : "   ";
                        Console.WriteLine($"- {status}{i.Key.ToString()}     |{itemMessage(i.Key)}");
                    }
                }
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("장착하고자 하는 아이템의 이름을 정확히 입력하세요.");

                string input = Console.ReadLine();

                if (Enum.TryParse(input, true, out Items selected))
                {
                    if (inventory.TryGetValue(selected, out int value))
                    {
                        if (value > 0)
                        {
                            EquipItem(selected, player);
                        }
                        else
                        {
                            Console.WriteLine("아이템이 없습니다.");
                            Thread.Sleep(500);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("제대로 입력해 주세요");
                    Thread.Sleep(500);
                }


            }

            public void ShowInvertory(Player player)
            {
                bool isAllItemEmpty = inventory.Values.All(x => x == 0);

                if (isAllItemEmpty == false)
                {
                    foreach (KeyValuePair<Items, int> i in inventory)
                    {
                        if (i.Value != 0)
                        {
                            Console.WriteLine($"{i.Key}는 {i.Value}개 있습니다.");
                        }
                    }

                    Console.WriteLine();
                    Console.WriteLine();

                    Console.WriteLine("2. 포션 사용 \n1. 장착 관리\n0. 나가기");
                    string input = Console.ReadLine();

                    if (input == "1" || input == "장착 관리" || input == "장착관리")
                    {
                        Equippage(player);

                    }
                    else if (input == "2" || input == "포션 사용" || input == "포션사용")
                    {
                        PotionPage(player);

                    }
                    else
                    {
                        Console.WriteLine("잘못된 입력입니다.");
                        Thread.Sleep(500);
                    }
                }
                else
                {
                    Console.WriteLine("아이템이 존재하지 않습니다");
                    Console.WriteLine("나가시려면 아무키나 눌러주세요");
                    string wantOut = Console.ReadLine();
                }
            }

            public void OpenStore(Player player)
            {
                Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
                Console.WriteLine();
                Console.WriteLine($"[보유 골드]\n{player.gold}");
                Console.WriteLine();
                Console.WriteLine("[아이템 목록]");
                Console.WriteLine();

                foreach (var product in store)
                {
                    string status = product.IsSold ? "구매 완료" : product.Price.ToString();
                    Console.WriteLine($"- {product.Item}  |{itemMessage(product.Item)}{status}");
                    Console.WriteLine();
                }


                //아이템 구매
                try
                {
                    Console.WriteLine("구매할 아이템을 적어주세요");
                    Console.WriteLine("나가기를 원하시면 나가기를 입력해주세요");
                    string input = Console.ReadLine();

                    switch (input)
                    {
                        case "수련자 갑옷":
                        case "수련자갑옷":
                            Buy(Items.수련자_갑옷, 1, player);
                            Thread.Sleep(500);
                            break;
                        case "무쇠갑옷":
                        case "무쇠 갑옷":
                            Buy(Items.무쇠갑옷, 1, player);
                            Thread.Sleep(500); break;
                        case "스파르타의 갑옷":
                        case "스파르타의갑옷":
                            Buy(Items.스파르타의_갑옷, 1, player);
                            Thread.Sleep(500); break;
                        case "낡은_검":
                        case "낡은 검":
                        case "낡은검":
                            Buy(Items.낡은_검, 1, player);
                            Thread.Sleep(500); break;
                        case "청동_도끼":
                        case "청동 도끼":
                        case "청동도끼":
                            Buy(Items.청동_도끼, 1, player);
                            Thread.Sleep(500); break;
                        case "스파르타의_창":
                        case "스파르타의 창":
                        case "스파르타의창":
                            Buy(Items.스파르타의_창, 1, player);
                            Thread.Sleep(500); break;
                        case "체력포션":
                        case "체력 포션":
                            Console.Write("구매할 수량을 입력해주세요 : ");
                            int countHealthPotion = int.Parse(Console.ReadLine());
                            Buy(Items.체력포션, countHealthPotion, player);
                            Thread.Sleep(500); break;
                        case "힘 포션":
                        case "힘포션":
                            Console.Write("구매할 수량을 입력해주세요 : ");
                            int countStrengthPotion = int.Parse(Console.ReadLine());
                            Buy(Items.체력포션, countStrengthPotion, player);
                            Thread.Sleep(500); break;
                        case "나가기":
                            Thread.Sleep(500);
                            break;

                    }
                }
                catch (KeyNotFoundException)
                {
                    Console.WriteLine("해당 아이템은 상점에 없습니다.");
                    Thread.Sleep(500);
                }


            }


        }

        public class Monster : ICharacter
        {
            public string Name { get; set; }
            public int Health { get; set; }
            public int Attack { get; set; }
            public int currentHP { get; set; }
            public bool IsDead => currentHP <= 0;

            public Monster(string name, int health, int attackPower)
            {
                Name = name;
                Health = health;
                currentHP = health * 2;
                Attack = new Random().Next(attackPower / 2, attackPower);
            }

            public void TakeDamage(int damage)
            {
                currentHP -= damage;
                if (IsDead == true)
                {
                    Console.WriteLine($"{Name}(이)가 죽었습니다.");
                }
                else
                {
                    Console.WriteLine($"{Name}(이)가 {damage}의 데미지를 입었습니다. \n남은 체력 : {currentHP}");
                }
            }
            public void GoldChange(int amount) { }
            public void GainExp(int amount) { }

        }

        public class Goblin : Monster
        {
            public Goblin(string name, int health, int attack) : base(name, health, attack) { }
        }

        public class Dragon : Monster
        {
            public Dragon(string name, int health, int attack) : base(name, health, attack) { }

        }

        public enum Items
        {
            수련자_갑옷,
            무쇠갑옷,
            스파르타의_갑옷,
            낡은_검,
            청동_도끼,
            스파르타의_창,
            체력포션,
            힘포션
        }

        public interface IItems
        {
            string Name { get; }


            void Use(Player player, Items item, int amount);
            void Get(Player player, Items item, int amount);
        }

        public class HealthPotion : IItems
        {
            private string name = Items.체력포션.ToString();
            public string Name => name;


            public void Use(Player player, Items item, int amount)
            {
                if (player.itemManager.isHaveItem[(int)item] == true)
                {
                    Console.WriteLine($"체력 포션을 {amount}개 사용합니다. 체력이 {50 * amount} 증가합니다.");
                    player.HPChange(50 + amount);
                    player.itemManager.ItemCountChange(item, -amount);
                }
                else
                {
                    Console.WriteLine("사용할 수 없습니다");
                }
            }

            public void Get(Player player, Items item, int amount)
            {
                player.itemManager.ItemCountChange(item, amount);
                Console.WriteLine("체력 포션을 획득했습니다.");
            }
        }

        public class StrengthPotion : IItems
        {
            private string name = Items.힘포션.ToString();
            public string Name => name;


            public void Use(Player player, Items item, int amount)
            {
                if (player.itemManager.isHaveItem[(int)item] == true)
                {
                    Console.WriteLine("힘포션을 사용합니다. 공격력이 10 증가합니다.");
                    player.AttackPower += 10;
                    player.itemManager.ItemCountChange(item, -amount);
                }
                else
                {
                    Console.WriteLine("사용할 수 없습니다");
                }
            }
            public void Get(Player player, Items item, int amount)
            {
                player.itemManager.ItemCountChange(item, amount);
                Console.WriteLine("힘포션을 획득했습니다.");
            }
        }

        public class Armor : IItems
        {
            public string Name { get; }
            public int DeffenPower { get; set; }


            public Armor(Items item, int armor)
            {
                this.Name = item.ToString();
                this.DeffenPower = armor;

            }

            public void Use(Player player, Items item, int amount)
            {
                player.itemManager.ItemCountChange(item, -amount);
            }

            public void Get(Player player, Items item, int amount)
            {
                player.itemManager.ItemCountChange(item, amount);
            }
        }
        //수련자 값옷
        public class Armor01 : Armor
        {
            public Armor01(Items name) : base(name, 5) { }
        }
        //무쇠갑옷
        public class Armor02 : Armor
        {
            public Armor02(Items name) : base(name, 9) { }
        }
        //스파르타의 갑옷
        public class Armor03 : Armor
        {
            public Armor03(Items name) : base(name, 15) { }
        }

        public class Weapon : IItems
        {
            public string Name { get; }
            public int AttackPower { get; set; }


            public Weapon(Items item, int AttackPower)
            {
                this.Name = item.ToString();
                this.AttackPower = AttackPower;

            }

            public void Use(Player player, Items item, int number)
            {
                player.itemManager.ItemCountChange(item, -number);
            }

            public void Get(Player player, Items item, int number)
            {
                player.itemManager.ItemCountChange(item, number);

            }
        }
        //낡은검
        public class Weapon01 : Weapon
        {
            public Weapon01(Items item) : base(item, 5) { }
        }
        //청동 동끼
        public class Weapon02 : Weapon
        {
            public Weapon02(Items item) : base(item, 9) { }
        }
        //낡은검
        public class Weapon03 : Weapon
        {
            public Weapon03(Items item) : base(item, 15) { }
        }

        //전투와 관련된 항목들을 관리하는 클래스입니다.
        public class Dungeon
        {
            private ICharacter player;
            private ICharacter monster;
            private List<IItems> rewards;
            private Items selection;

            public delegate void GameEvent(ICharacter character);
            public event GameEvent OnCharacterDeath;

            public Dungeon(ICharacter player, ICharacter monster, List<IItems> rewards)
            {
                this.player = player;
                this.monster = monster;
                this.rewards = rewards;
                OnCharacterDeath += DungeonClear;
            }


            public void DungeonStart()
            {
                Console.WriteLine($"던전에 입장하였습니다.\n[당신의 정보]\n공격력 : {player.Attack}\n체력 : {player.currentHP}");
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine($"[몬스터의 정보]\n몬스터 이름: {monster.Name}\n공격력: {monster.Attack}\n체력: {monster.currentHP}");
                Console.WriteLine();
                Console.WriteLine("5초 후에 전투가 시작됩니다.");
                Thread.Sleep(5000);

                while (!player.IsDead && !monster.IsDead)
                {
                    Console.WriteLine($"{player.Name}의 차례입니다.");
                    monster.TakeDamage(player.Attack);
                    Console.WriteLine();
                    Thread.Sleep(1500);

                    if (monster.IsDead) break;

                    Console.WriteLine($"{monster.Name}의 차례입니다.");
                    player.TakeDamage(monster.Attack);
                    Console.WriteLine();
                    Thread.Sleep(1500);

                    if (player.IsDead) break;
                }

                if (player.IsDead)
                {
                    OnCharacterDeath?.Invoke(player);
                }
                else if (monster.IsDead)
                {
                    OnCharacterDeath?.Invoke(monster);
                }
            }


            private void DungeonClear(ICharacter character)
            {
                if (character is Monster)
                {
                    Console.Clear();
                    Console.WriteLine($"{character.Name}을 물리쳤습니다.");
                    Console.WriteLine();

                    if (rewards != null)
                    {
                        while (true)
                        {
                            Console.WriteLine("아래의 보상 아이템 중 하나를 선택하여 사용할 수 있습니다:");
                            Console.WriteLine();
                            foreach (var reward in rewards)
                            {
                                Console.WriteLine($"{reward.Name}");
                            }
                            Console.WriteLine();
                            Console.WriteLine("획득할 아이템 이름을 입력하세요:");
                            string input = Console.ReadLine();


                            //아이템을 모두 1로 

                            IItems selectedItem = rewards.Find(item => item.Name == input);
                            if (selectedItem != null && Enum.TryParse(input, true, out Items selected))
                            {
                                selectedItem.Get((Player)player, selected, 1);
                                //체력포션을 얻었으면
                                player.GainExp(100);
                                player.GoldChange(1000);
                                Console.Clear();
                                Console.WriteLine($"{selected}를 인벤토리에 추가합니다.");
                                Console.WriteLine("1000골드와 100의 경험치를 획득하였습니다.");
                                Thread.Sleep(3000);
                                break;
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("다시 입력해주세요");
                                Console.WriteLine();
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("게임 오버!");
                }



            }

            public void DungeonSelect()
            {
                Console.WriteLine("던전은 총 5단계로 이루어집니다.");
            }
        }



        static void Main(string[] args)
        {

            //게임 시작
            Console.WriteLine("RPG에 오신걸 환영합니다");

            string userName;
            while (true)
            {
                Console.WriteLine("당신의 이름을 입력해주세요.");

                string input = Console.ReadLine();
                Console.WriteLine($"{input}이 맞다면 '예' 아니면 '아니오'를 입력해주세요");
                string yesorno = Console.ReadLine();

                if (input != null && yesorno == "예")
                {
                    userName = input;
                    Console.Clear();
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("다시 입력해주세요");
                }
            }



            Player player = new Player(userName);
            ItemManager itemManager = new ItemManager();
            SeceneManager sceneManager = new SeceneManager(player, itemManager);
            Goblin goblin = new Goblin("고블린", 50, 5);
            Goblin assassinGoblin = new Goblin("고블린 암살자", 50, 15);
            Goblin kingGoblin = new Goblin("킹 고블린", 200, 20);
            Dragon hatchling = new Dragon("해츨링", 300, 30);
            Dragon blackDragon = new Dragon("블랙 드래곤", 500, 50);


            List<IItems> stage1Rewards = new List<IItems> { new HealthPotion(), new StrengthPotion() };
            List<IItems> stage2Rewards = new List<IItems> { new HealthPotion(), new StrengthPotion() };
            List<IItems> stage3Rewards = new List<IItems> { new HealthPotion(), new StrengthPotion(), new Weapon01(Items.낡은_검), new Armor01(Items.수련자_갑옷) };
            List<IItems> stage4Rewards = new List<IItems> { new HealthPotion(), new StrengthPotion(), new Weapon02(Items.청동_도끼), new Armor02(Items.무쇠갑옷) };
            List<IItems> stage5Rewards = new List<IItems> { new HealthPotion(), new StrengthPotion(), new Weapon03(Items.스파르타의_창), new Armor03(Items.스파르타의_갑옷) };

            Dungeon dungeon01 = new Dungeon(player, goblin, stage1Rewards);
            Dungeon dungeon02 = new Dungeon(player, assassinGoblin, stage2Rewards);
            Dungeon dungeon03 = new Dungeon(player, kingGoblin, stage3Rewards);
            Dungeon dungeon04 = new Dungeon(player, hatchling, stage4Rewards);
            Dungeon dungeon05 = new Dungeon(player, blackDragon, stage5Rewards);

            sceneManager.AddDungeon(dungeon01);
            sceneManager.AddDungeon(dungeon02);
            sceneManager.AddDungeon(dungeon03);
            sceneManager.AddDungeon(dungeon05);
            sceneManager.AddDungeon(dungeon04);


            //게임 루프
            while (true)
            {
                sceneManager.GameMenu(player);
            }


        }
    }

}





