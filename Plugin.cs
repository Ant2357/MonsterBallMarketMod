using System.Linq;
using BepInEx;
using HarmonyLib;

[BepInPlugin("ant2357.monsterball_market_mod", "Monster Ball Market Mod", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    public void OnStartCore()
    {
        var harmony = new Harmony("ant2357.monsterball_market_mod");
        harmony.PatchAll();
    }
}

[HarmonyPatch(typeof(Trait), "OnBarter")]
public static class MonsterBallMarket
{
    [HarmonyPostfix]
    public static void Postfix(ref Trait __instance)
    {
        // 道具屋・何でも屋・セールスマンのみ対象とする
        if (__instance.owner == null || (__instance.owner.id != "merchant" && __instance.owner.id != "merchant_general" && __instance.owner.id != "merchat_cyber")) return;

        // 在庫補充時のみ処理を行う
        if (!__instance.owner.isRestocking) return;

        // 商人のチェストを取得または作成
        Thing chest = __instance.owner.things.Find("chest_merchant", -1, -1);
        if (chest == null)
        {
            chest = ThingGen.Create("chest_merchant", -1, -1);
            __instance.owner.AddThing(chest, true, -1, -1);
        }
        Card chestCard = (Card)chest;

        // 既にモンスターボールが存在するか確認
        bool hasMonsterBall = chestCard.things.Any(item => item.id == "monsterball");

        // 既にモンスターボールが存在する場合は処理を終了
        if (hasMonsterBall) return;

        // モンスターボールを追加
        Thing monsterBall = ThingGen.Create("monsterball", -1, __instance.ShopLv + 20);
        monsterBall.SetNum(2);
        chestCard.AddThing(monsterBall, true, -1, -1);
    }
}
