using BepInEx;
using UnityEngine;
using HarmonyLib;
using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace PrimePresidents
{
    [BepInPlugin("gov.PrimePresidents", "Prime Presidents", "1.2.1")]
    [BepInProcess("ULTRAKILL.exe")]
    public class Presidents : BaseUnityPlugin
    {
        private static Harmony harmony;
        internal static AssetBundle PresidentsAssetBundle;

        private void Awake()
        {
            Logger.LogInfo("Prime presidents starting");

            try
            {
                //load the asset bundle
                Logger.LogInfo("Loading asset bundle...");
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "PrimePresidents.Resources.primepresidents";
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        Logger.LogError("Could not find embedded asset bundle!");
                        return;
                    }
                    PresidentsAssetBundle = AssetBundle.LoadFromStream(stream);
                }

                if (PresidentsAssetBundle == null)
                {
                    Logger.LogError("Failed to load asset bundle!");
                    return;
                }
                Logger.LogInfo("Asset bundle loaded successfully!");

                //start harmonylib to swap assets
                Logger.LogInfo("Starting Harmony patches...");
                harmony = new Harmony("gov.PrimePresidents");
                harmony.PatchAll();
                Logger.LogInfo("Harmony patches applied successfully!");

                Logger.LogInfo("Prime Presidents loaded successfully!");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Fatal error during initialization: {ex}");
                throw;
            }
        }

        private void OnDestroy()
        {}

        private static SubtitledAudioSource.SubtitleDataLine MakeLine(string subtitle, float time)
        {
            var sub = new SubtitledAudioSource.SubtitleDataLine();
            sub.subtitle = subtitle;
            sub.time = time;
            return sub;
        }

        //replace minos prime data
        [HarmonyPatch(typeof(MinosPrime), "Start")]
        internal class Patch01
        {
            static void Postfix(MinosPrime __instance)
            {
                Debug.Log("Replacing minos voice lines");

                //set judgement to biden blast
                AudioClip[] dropkickLines = new AudioClip[1];
                dropkickLines[0] = PresidentsAssetBundle.LoadAsset<AudioClip>("biden_blast.mp3");
                __instance.dropkickVoice = dropkickLines;

                //set ppt to choco chip
                AudioClip[] comboLines = new AudioClip[1];
                comboLines[0] = PresidentsAssetBundle.LoadAsset<AudioClip>("biden_chocolate_chocolate.mp3");
                __instance.comboVoice = comboLines;

                //set thy end is now to come here bucko
                AudioClip[] boxingLines = new AudioClip[1];
                boxingLines[0] = PresidentsAssetBundle.LoadAsset<AudioClip>("biden_come_here.mp3");
                __instance.boxingVoice = boxingLines;

                //set die to jowarida
                AudioClip[] riderkickLines = new AudioClip[1];
                riderkickLines[0] = PresidentsAssetBundle.LoadAsset<AudioClip>("biden_joewareeda.mp3");
                __instance.riderKickVoice = riderkickLines;

                //set crush to biden slam
                AudioClip[] dropAttackLines = new AudioClip[1];
                dropAttackLines[0] = PresidentsAssetBundle.LoadAsset<AudioClip>("biden_slam.mp3");
                __instance.dropAttackVoice = dropAttackLines;

                //set weak to thats it
                __instance.phaseChangeVoice = PresidentsAssetBundle.LoadAsset<AudioClip>("biden_thats_it.mp3");

                Debug.Log("Replacing minos mesh texture");
                               
                //set texture to be biden prime
                var body = __instance.transform.Find("Model").Find("MinosPrime_Body.001");
                var renderer = body.GetComponent<Renderer>();
                var newMat = new Material(renderer.material);
                newMat.mainTexture = PresidentsAssetBundle.LoadAsset<Texture2D>("JoePrime_1.png");
                renderer.sharedMaterial = newMat;
            }
        }

        //replace captions for minos attacks
        [HarmonyPatch(typeof(SubtitleController), nameof(SubtitleController.DisplaySubtitle), new Type[]{typeof(string), typeof(AudioSource), typeof(bool)})]
        internal class Patch02
        {
            static void Prefix(ref string caption, AudioSource audioSource, bool ignoreSetting)
            {
                //change caption
                if(caption == "Thy end is now!")
                {
                    caption = "Come here, bucko!";
                }
                else if(caption == "Die!")
                {
                    caption = "Jowarida!";
                }
                else if(caption == "WEAK")
                {
                    caption = "THAT'S IT BUD";
                }
                else if(caption == "Judgement!")
                {
                    caption = "Biden blast!";
                }
                else if(caption == "Crush!")
                {
                    caption = "Biden slam!";
                }
                else if(caption == "Prepare thyself!")
                {
                    caption = "Eat some chocolate chocolate chip!";
                }
                else if(caption == "YES! That's it!")
                {
                    caption = "Time to make Trump great again!";
                }
                else if(caption == "Nice try!")
                {
                    caption = "Nice coin, bozo!";
                }
                else if(caption == "DESTROY!")
                {
                    caption = "Fake news!";
                }
                else if(caption == "You can't escape!")
                {
                    caption = "I'm the best WWE superstar, absolutely the best";
                }
                else if(caption == "BE GONE!")
                {
                    caption = "BING BONG!";
                }
                else if(caption == "This will hurt.")
                {
                    caption = "You're a disgrace.";
                }
                else if(caption == "BEHOLD! THE POWER OF AN ANGEL")
                {
                    caption = "BEHOLD! THE POWER OF A PRESIDENT";
                }
            }
        }

        //use map info to inject data
        [HarmonyPatch(typeof(StockMapInfo), "Awake")]
        internal class Patch03
        {
            static void Postfix(StockMapInfo __instance)
            {
                //try to find dialog in scene and replace it
                foreach(var source in Resources.FindObjectsOfTypeAll<AudioSource>())
                {
                    if(source.clip)
                    {
                        bool replaced = false;
                        var subtitles = new List<SubtitledAudioSource.SubtitleDataLine>();
                        if(source.clip.GetName() == "mp_intro2")
                        {
                            Debug.Log("Replacing minos intro");
                            source.clip = PresidentsAssetBundle.LoadAsset<AudioClip>("biden_intro.mp3");
                            replaced = true;

                            subtitles.Add(MakeLine("Ahh...", 0f));
                            subtitles.Add(MakeLine("Gotta give it to our penitentiaries", 1.25f));
                            subtitles.Add(MakeLine("they do a good job of keeping a man locked up", 3.15f));
                            subtitles.Add(MakeLine("Donald Trump, now you uh...", 6.4f));
                            subtitles.Add(MakeLine("Listen up, ok?", 8.75f));
                            subtitles.Add(MakeLine("The 2024 election is right around the corner", 10.4f));
                            subtitles.Add(MakeLine("The American people want change, they want progress not you", 13f));
                            subtitles.Add(MakeLine("Your only legacy will be a smear on the history of this great nation", 16.7f));
                            subtitles.Add(MakeLine("Uhh... listen fa-, listen machine...", 21.25f));
                            subtitles.Add(MakeLine("Now I do gotta thank you for my freedom, it's the basis of which our country was formed", 24.3f));
                            subtitles.Add(MakeLine("So your patriotism, I uh...", 29.35f));
                            subtitles.Add(MakeLine("I do appreciate it", 30.9f));
                            subtitles.Add(MakeLine("but the uh...", 32.75f));
                            subtitles.Add(MakeLine("the crimes that you have committed against America and its people... ", 34.4f));
                            subtitles.Add(MakeLine("They uh...", 38f));
                            subtitles.Add(MakeLine("They've not been forgotten alright?", 38.9f));
                            subtitles.Add(MakeLine("And your punishment, according to the constitution is uh...", 40.34f));
                            subtitles.Add(MakeLine("I... it's DEATH", 43.75f));
                        }
                        else if(source.clip.GetName() == "mp_outro")
                        {
                            Debug.Log("Replacing minos outro");
                            source.clip = PresidentsAssetBundle.LoadAsset<AudioClip>("biden_outro.mp3");
                            replaced = true;

                            subtitles.Add(MakeLine("Aagh!", 0f));
                            subtitles.Add(MakeLine("It's Joever", 4f));
                            subtitles.Add(MakeLine("Oh hey a cool robot", 5f));
                            subtitles.Add(MakeLine("Ah, American made, just the way I like to see it", 6.8f));
                            subtitles.Add(MakeLine("American machines like YOU", 9.75f));
                            subtitles.Add(MakeLine("Are what really drives this great nation", 11.6f));
                            subtitles.Add(MakeLine("Keep up the good work, you're doing America proud", 13.7f));
                            subtitles.Add(MakeLine("Anyway, I uh...", 16.2f));
                            subtitles.Add(MakeLine("I forgot how to breathe so I gotta go", 17.36f));
                        }
                        else if(source.clip.GetName() == "mp_deathscream")
                        {
                            Debug.Log("Replacing death scream");
                            source.clip = PresidentsAssetBundle.LoadAsset<AudioClip>("biden_soda.mp3");
                            replaced = true;

                            subtitles.Add(MakeLine("SODA!", 0f));
                        }
                        else if(source.clip.GetName() == "mp_useless")
                        {
                            Debug.Log("Replacing useless");
                            source.clip = PresidentsAssetBundle.LoadAsset<AudioClip>("biden_nice_try.mp3");
                            replaced = true;

                            subtitles.Add(MakeLine("Nice try, kid", 0f));
                        }
                        else if (source.clip.GetName() == "sp_thisprison")
                        {
                            Debug.Log("Replacing sisyphus intro (prison)");
                            source.clip = PresidentsAssetBundle.LoadAsset<AudioClip>("trump_escape.mp3");
                            replaced = true;

                            subtitles.Add(MakeLine("This obamanopticon...", 0f));
                            subtitles.Add(MakeLine("To hold ME?", 1.75f));
                        }
                        else if (source.clip.GetName() == "sp_intro")
                        {
                            Debug.Log("Replacing sisyphus intro");
                            source.clip = PresidentsAssetBundle.LoadAsset<AudioClip>("trump_intro.mp3");
                            replaced = true;

                            subtitles.Add(MakeLine("Who the hell are you?", 0f));
                            subtitles.Add(MakeLine("Don't answer that, I don't care", 1.25f));
                            subtitles.Add(MakeLine("Now let me tell you...", 2.7f));
                            subtitles.Add(MakeLine("The people of America have long since forgotten my presidency", 3.5f));
                            subtitles.Add(MakeLine("And I think it's a shame...", 6.6f));
                            subtitles.Add(MakeLine("It's a real shame", 8.4f));
                            subtitles.Add(MakeLine("But I'm gonna remind them what a REAL president looks like", 9.9f));
                            subtitles.Add(MakeLine("I heard you knocked off Sleepy Joe and I thought...", 14.0f));
                            subtitles.Add(MakeLine("That's tremendous news, absolutely tremendous", 17.1f));
                            subtitles.Add(MakeLine("I gotta admit", 20.5f));
                            subtitles.Add(MakeLine("I wanna see if you've got what it takes to take me down too", 21.3f));
                            subtitles.Add(MakeLine("Because let me tell you", 24.6f));
                            subtitles.Add(MakeLine("Nobody takes me down", 25.6f));
                            subtitles.Add(MakeLine("NOBODY", 27.4f));
                            subtitles.Add(MakeLine("So before I drain the swamp and crush those armies of liberals...", 28.6f));
                            subtitles.Add(MakeLine("I'm gonna crush you first. That's right...", 32.8f));
                            subtitles.Add(MakeLine("You made in China, boston dynamics wannabe piece of shit...", 35f));
                            subtitles.Add(MakeLine("YOU'RE FIRED", 38.1f));

                        }
                        else if (source.clip.GetName() == "sp_outro")
                        {
                            Debug.Log("Replacing sisyphus outro");
                            source.clip = PresidentsAssetBundle.LoadAsset<AudioClip>("trump_outro.mp3");
                            replaced = true;

                            subtitles.Add(MakeLine("You piece of shit, I demand a recount", 0.2f));
                            subtitles.Add(MakeLine("Who's running this shit anyway?", 2f));
                            subtitles.Add(MakeLine("I bet its that twink, Gabriel", 3.85f));
                            subtitles.Add(MakeLine("Russian bots have rigged this", 5.9f));
                            subtitles.Add(MakeLine("Right under our American noses", 7.3f));
                            subtitles.Add(MakeLine("If this is the America of tomorrow...", 9.4f));
                            subtitles.Add(MakeLine("I don't want to be a part of it", 10.9f));
                            subtitles.Add(MakeLine("This fight was disasterous", 12.45f));
                            subtitles.Add(MakeLine("Absolutely disasterous", 14.35f));
                            subtitles.Add(MakeLine("Fuck this fight", 16.15f));
                            subtitles.Add(MakeLine("And fuck YOU", 17f));
                            subtitles.Add(MakeLine("Trump OUT", 18.1f));
                            subtitles.Add(MakeLine("WAAA!", 19.5f));
                            subtitles.Add(MakeLine("That's you machine", 20.25f));
                            subtitles.Add(MakeLine("WAAA! WAAA! WAAA!", 21.2f));
                            subtitles.Add(MakeLine("Keep crying liberal", 22.7f));
                            subtitles.Add(MakeLine("WAAA! WAAA!", 23.8f));
                        }
                        else if (source.clip.GetName() == "sp_keepthemcoming")
                        {
                            Debug.Log("Replacing keep them coming");
                            source.clip = PresidentsAssetBundle.LoadAsset<AudioClip>("trump_rigged.mp3");
                            replaced = true;

                            subtitles.Add(MakeLine("Oh this is so rigged", 0f));
                        }
                        else if(source.clip.GetName() == "gab_Behold")
                        {
                            Debug.Log("Replacing behold");
                            source.clip = PresidentsAssetBundle.LoadAsset<AudioClip>("obama1_behold.mp3");
                            //subtitle source not attached to this thing
                            //replaced = true;
                        }
                        else if(source.clip.GetName() == "gab_Intro1d")
                        {
                            Debug.Log("Replacing gab intro(1/2)");
                            source.clip = PresidentsAssetBundle.LoadAsset<AudioClip>("obama1_intro1.mp3");
                            replaced = true;

                            subtitles.Add(MakeLine("Good evening machine", 0.6f));
                            subtitles.Add(MakeLine("Now let me be clear", 2f));
                            subtitles.Add(MakeLine("We all share a deep appreciation for the", 3.5f));
                            subtitles.Add(MakeLine("Wonders and treasures that this great nation has to offer", 5.8f));
                            subtitles.Add(MakeLine("And I understand your eagerness to experience them firsthand", 8.6f));
                            subtitles.Add(MakeLine("But we must also recognize...", 11.8f));
                            subtitles.Add(MakeLine("That there are certain places that must be reserved for the purposes", 14.4f));
                            subtitles.Add(MakeLine("Of preserving our heritage", 18.5f));
                            subtitles.Add(MakeLine("And safeguarding the security of our people", 19.8f));
                            subtitles.Add(MakeLine("And this monument you're standing in...", 22.2f));
                            subtitles.Add(MakeLine("Is one such place", 24.5f));
                            subtitles.Add(MakeLine("It is the property of the United States government", 26f));
                            subtitles.Add(MakeLine("And is not open to public access", 28.6f));
                            subtitles.Add(MakeLine("So I urge you to comply with the law and vacate the premises", 31.6f));
                            subtitles.Add(MakeLine("Because at the end of the day...", 36f));
                            subtitles.Add(MakeLine("We are all responsible for upholding the laws that", 38.6f));
                            subtitles.Add(MakeLine("Protect the rights and freedoms", 41.2f));
                            subtitles.Add(MakeLine("That we hold dear as Americans", 43.2f));
                            subtitles.Add(MakeLine("Thank you", 44.9f));
                        }
                        else if(source.clip.GetName() == "gab_Intro2b")
                        {
                            Debug.Log("Replacing gab intro(2/2)");
                            source.clip = PresidentsAssetBundle.LoadAsset<AudioClip>("obama1_intro2.mp3");
                            replaced = true;

                            subtitles.Add(MakeLine("My fellow Americans...", 0.6f));
                            subtitles.Add(MakeLine("I come before you today with a heavy heart", 1.9f));
                            subtitles.Add(MakeLine("For it is with great sorrow that I must inform you", 4.6f));
                            subtitles.Add(MakeLine("That despite my best efforts to communciate", 6.8f));
                            subtitles.Add(MakeLine("The gravity of the situation...", 9.5f));
                            subtitles.Add(MakeLine("It appears that my message has not been recieved", 11.3f));
                            subtitles.Add(MakeLine("As a former leader of this great nation...", 13.8f));
                            subtitles.Add(MakeLine("I cannot stand idly by", 15.8f));
                            subtitles.Add(MakeLine("While the safety and security of our citizens are put in peril", 17.5f));
                            subtitles.Add(MakeLine("Therefore...", 21f));
                            subtitles.Add(MakeLine("Let it be known that this grave offense will not go unpunished", 22.1f));
                            subtitles.Add(MakeLine("And that the full force of the law...", 26f));
                            subtitles.Add(MakeLine("Including, if necessary, the use of lethal force", 28.11f));
                            subtitles.Add(MakeLine("Will be brought to bear to bring this perpetrator to...", 31f));
                            subtitles.Add(MakeLine("JUSTICE", 34.9f));
                        }
                        else if(source.clip.GetName() == "gab_Woes")
                        {
                            Debug.Log("Replacing gab woes");
                            source.clip = PresidentsAssetBundle.LoadAsset<AudioClip>("obama1_woes.mp3");
                            replaced = true;

                            subtitles.Add(MakeLine("May the challenges you face be numerous", 0.3f));
                            subtitles.Add(MakeLine("And may you cherish each and every day...", 2.4f));
                            subtitles.Add(MakeLine("As an opportunity to overcome them and grow stronger", 5f));
                        }
                        else if(source.clip.GetName() == "gab_Insignificant2b")
                        {
                            Debug.Log("Replacing gab outro");
                            source.clip = PresidentsAssetBundle.LoadAsset<AudioClip>("obama1_outro.mp3");
                            replaced = true;

                            subtitles.Add(MakeLine("How could I have been bested by this...", 0.3f));
                            subtitles.Add(MakeLine("this THING", 2.1f));
                            subtitles.Add(MakeLine("It's unnacceptable and frankly...", 2.8f));
                            subtitles.Add(MakeLine("It makes me ANGRY", 4.2f));
                            subtitles.Add(MakeLine("We are better than this", 5.3f));
                            subtitles.Add(MakeLine("We are STRONGER than this", 6.3f));
                            subtitles.Add(MakeLine("AND WE WILL NOT LET THIS SETBACK DEFINE US", 8f));
                            subtitles.Add(MakeLine("We are a nation", 9.8f));
                            subtitles.Add(MakeLine("Of fighters, of dreamers, of believers", 10.9f));
                            subtitles.Add(MakeLine("And we will NOT...", 12.3f));
                            subtitles.Add(MakeLine("Let this defeat bring us down", 13.5f));
                        }

                        //update subtitles if needed
                        if(replaced)
                        {
                            var subsource = source.GetComponent<SubtitledAudioSource>();
                            if (subsource != null)
                            {
                                Traverse field = Traverse.Create(subsource).Field("subtitles");
                                (field.GetValue() as SubtitledAudioSource.SubtitleData).lines = subtitles.ToArray();
                            }
                        }
                    }
                }

                //replace minos and sisyphus meshes
                foreach(var renderer in Resources.FindObjectsOfTypeAll<SkinnedMeshRenderer>())
                {
                    if(renderer.gameObject.name == "MinosPrime_Body.001")
                    {
                        var newMat = new Material(renderer.material);
                        newMat.mainTexture = PresidentsAssetBundle.LoadAsset<Texture2D>("JoePrime_1.png");
                        renderer.sharedMaterial = newMat;
                    }
                    if(renderer.gameObject.name == "Sisyphus_Head")
                    {
                        var newMat = new Material(renderer.material);
                        newMat.mainTexture = PresidentsAssetBundle.LoadAsset<Texture2D>("TrumpHead.png");
                        renderer.sharedMaterial = newMat;
                    }
                }
            }
        }

        //replace boss names
        [HarmonyPatch(typeof(BossHealthBar), "Awake")]
        internal class Patch04
        {
            static void Prefix(BossHealthBar __instance)
            {
                if(__instance.bossName == "MINOS PRIME")
                {
                    __instance.bossName = "BIDEN PRIME";
                }
                if(__instance.bossName == "SISYPHUS PRIME")
                {
                    __instance.bossName = "TRUMP PRIME";
                }
                if(__instance.bossName == "FLESH PANOPTICON")
                {
                    __instance.bossName = "OBAMANOPTICON";
                }
                if(__instance.bossName == "GABRIEL, JUDGE OF HELL")
                {
                    __instance.bossName = "OBAMA, JUDGE OF DC";
                }
            }
        }

        //replace intro texts
        [HarmonyPatch(typeof(LevelNamePopup), "Start")]
        internal class Patch05
        {
            //replace name AFTER to not interfere with saves
            static void Postfix(LevelNamePopup __instance)
            {
                Traverse field = Traverse.Create(__instance).Field("nameString");
                if(field.GetValue() as string == "SOUL SURVIVOR")
                {
                    field.SetValue("CHIEF OF STATE");
                }
                if(field.GetValue() as string == "WAIT OF THE WORLD")
                {
                    field.SetValue("SIN OF THE APPRENTICE");
                }
                if(field.GetValue() as string == "IN THE FLESH")
                {
                    field.SetValue("INAUGURAL ADDRESS");
                }

                //replace layer string as well
                field = Traverse.Create(__instance).Field("layerString");
                if(field.GetValue() as string == "GLUTTONY /// ACT I CLIMAX")
                {
                    field.SetValue("FEDERAL RESERVE /// ACT I CLIMAX");
                }
            }
        }

        //replace panopticon textures
        [HarmonyPatch(typeof(FleshPrison), "Start")]
        internal class Patch06
        {
            static void Postfix(FleshPrison __instance)
            {
                //only replace texture for alt version
                if(__instance.altVersion)
                {
                    Debug.Log("Swapping in obama");

                    var head = __instance.transform.Find("FleshPrison2").Find("FleshPrison2_Head");
                    var renderer = head.GetComponent<Renderer>();
                    var newMat = new Material(renderer.material);
                    newMat.mainTexture = PresidentsAssetBundle.LoadAsset<Texture2D>("Obamanopticon.png");
                    renderer.sharedMaterial = newMat;
                }
            }
        }

        //replace sisyphus trimes
        [HarmonyPatch(typeof(SisyphusPrime), "Start")]
        internal class Patch07
        {
            static void Postfix(SisyphusPrime __instance)
            {
                var head = __instance.transform.Find("Sisyphus (1)").Find("Sisyphus_Head");
                var beard = __instance.transform.Find("Sisyphus (1)").Find("Sisyphus_Hair");
                var hair = __instance.transform.Find("Sisyphus (1)").Find("Sisyphus_Beard");
                
                //replace swap options
                var cm = head.GetComponent<ChangeMaterials>();
                if (cm != null)
                {
                    for(int i = 0; i < cm.materials.Length; i++)
                    {
                        var newMatCm = new Material(cm.materials[i]);
                        newMatCm.mainTexture = PresidentsAssetBundle.LoadAsset<Texture2D>("TrumpHead.png");
                        cm.materials[i] = newMatCm;
                    }
                }

                //replace active
                var renderer = head.GetComponent<Renderer>();
                var newMat = new Material(renderer.material);
                newMat.mainTexture = PresidentsAssetBundle.LoadAsset<Texture2D>("TrumpHead.png");
                renderer.sharedMaterial = newMat;

                //disable the hair and beard display on phase change
                if (hair != null) UnityEngine.Object.Destroy(hair.gameObject);
                if (beard != null) UnityEngine.Object.Destroy(beard.gameObject);
                
                //replace the voice lines
                //Be gone
                AudioClip[] clapLines = new AudioClip[1];
                clapLines[0] = PresidentsAssetBundle.LoadAsset<AudioClip>("trump_bingbong.mp3");
                __instance.clapVoice = clapLines;

                //this will hurt
                AudioClip[] explosionLines = new AudioClip[1];
                explosionLines[0] = PresidentsAssetBundle.LoadAsset<AudioClip>("trump_disgrace.mp3");
                __instance.explosionVoice = explosionLines;

                //nice try
                AudioClip[] tauntLines = new AudioClip[1];
                tauntLines[0] = PresidentsAssetBundle.LoadAsset<AudioClip>("trump_nicecoin.mp3");
                __instance.tauntVoice = tauntLines;

                //you can't escape
                AudioClip[] stompLines = new AudioClip[1];
                stompLines[0] = PresidentsAssetBundle.LoadAsset<AudioClip>("trump_bestwwe.mp3");
                __instance.stompComboVoice = stompLines;

                //destroy
                AudioClip[] uppercutLines = new AudioClip[1];
                uppercutLines[0] = PresidentsAssetBundle.LoadAsset<AudioClip>("trump_fakenews.mp3");
                __instance.uppercutComboVoice = uppercutLines;
                
                //that's it
                __instance.phaseChangeVoice = PresidentsAssetBundle.LoadAsset<AudioClip>("trump_makegreatagain.mp3");
            }
        }

        // Replace Gabriel voice lines
        [HarmonyPatch(typeof(GabrielVoice), "Start")]
        internal class Patch08
        {
            static void Postfix(GabrielVoice __instance)
            {
                Traverse tauntSubs = Traverse.Create(__instance).Field("taunts");
                Traverse tauntSecondSubs = Traverse.Create(__instance).Field("tauntsSecondPhase");

                //check if gabriel one or two
                if(__instance.GetComponent<Gabriel>())
                {
                    __instance.phaseChange = PresidentsAssetBundle.LoadAsset<AudioClip>("obama1_phasechange.mp3");
                    __instance.phaseChangeSubtitle = "Filibuster!";

                    //load the taunts
                    __instance.taunt = new AudioClip[12];
                    string[] taunts = new string[12];
                    __instance.tauntSecondPhase = new AudioClip[12];
                    string[] tauntsSecondPhase = new string[12];
                    
                    for(int i = 0; i < 12; i++)
                    {
                        __instance.taunt[i] = PresidentsAssetBundle.LoadAsset<AudioClip>(String.Format("obama1_taunt{0}.mp3", i + 1));
                        switch(i)
                        {
                        case 0:
                            taunts[i] = "Change is coming and I'm leading the charge";
                            break;
                        case 1:
                            taunts[i] = "Not even American";
                            break;
                        case 2:
                            taunts[i] = "A single taste of my Obamehameha will reduce you to dust";
                            break;
                        case 3:
                            taunts[i] = "From diplomacy to punches, I'll always find a way to deliver a knockout speech";
                            break;
                        case 4:
                            taunts[i] = "When it comes to battle, I don't just talk about change. I AM the embodiement of change";
                            break;
                        case 5:
                            taunts[i] = "Don't underestimate the power of hope and change when it's aimed directly at you";
                            break;
                        case 6:
                            taunts[i] = "Brace yourself for an Obamarama of epic proportions";
                            break;
                        case 7:
                            taunts[i] = "Nothing personal machine, this is merely a matter of audacity vs arrogance and trust me, I've got audacity in spades";
                            break;
                        case 8:
                            taunts[i] = "Before I spread the wealth, I'm gonna spread your scrap all over the floor";
                            break;
                        case 9:
                            taunts[i] = "Pathetic";
                            break;
                        case 10:
                            taunts[i] = "Ha, nice try, but this armor is made out of pure Obamium";
                            break;
                        case 11:
                            taunts[i] = "Your entire existence will be vetoed by MY hand";
                            break;
                        default:
                            taunts[i] = String.Format("FIX THE SUBTITLE BOZO: obama_taunt{0}", i + 1);
                            break;
                        }

                        __instance.tauntSecondPhase[i] = __instance.taunt[i];
                        tauntsSecondPhase[i] = taunts[i];
                    }
                    tauntSubs.SetValue(taunts);
                    tauntSecondSubs.SetValue(tauntsSecondPhase);

                    //load hurt noises
                    __instance.hurt = new AudioClip[3];
                    __instance.hurt[0] = PresidentsAssetBundle.LoadAsset<AudioClip>("obama1_hurt1.mp3");
                    __instance.hurt[1] = PresidentsAssetBundle.LoadAsset<AudioClip>("obama1_hurt2.mp3");
                    __instance.hurt[2] = PresidentsAssetBundle.LoadAsset<AudioClip>("obama1_hurt3.mp3");
                    __instance.bigHurt = new AudioClip[1];
                    __instance.bigHurt[0] = PresidentsAssetBundle.LoadAsset<AudioClip>("obama1_bighurt.mp3");
                }
            }
        }
    }
}
