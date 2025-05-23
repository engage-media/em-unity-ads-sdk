package com.engage.unityvideoplugin

import android.app.Activity
import android.util.Log
import android.view.View
import android.view.ViewGroup
import com.engage.engageadssdk.EMAdView
import com.engage.engageadssdk.EMVideoPlayerListener
import com.engage.engageadssdk.module.EMAdsModule
import com.engage.engageadssdk.module.EMAdsModuleInput
import com.engage.engageadssdk.module.EMAdsModuleInputBuilder
import com.engage.engageadssdk.ui.EmClientContentController
import com.unity3d.player.UnityPlayer
import org.json.JSONObject

class EMAdsPlugin {

    private lateinit var emAdView: EMAdView
    private val emAdsModuleInput: EMAdsModuleInput by lazy { EMAdsModule.getInstance() }

    fun initialize(activity: Activity, adsConfigJsonString: String) {

        try {
            val config: JSONObject = JSONObject(adsConfigJsonString)

            // Initialize the Engage SDK using the parsed config
            EMAdsModule.init(
                    EMAdsModuleInputBuilder()
                            .isDebug(config.optBoolean("_isTestMode", false))
                            .bundleId(config.optString("_bundleId", ""))
                            .publisherId(
                                    config.optString("_publisherId")
                                            ?: run { error("publisherId is required") }
                            )
                            .channelId(
                                    config.optString("_channelId")
                                            ?: run { error("channelId is required") }
                            )
                            .isAutoPlay(config.optBoolean("_isAutoplay", false))
                            .baseUrl(config.optString("_baseUrl", ""))
                            .isGdprApproved(false)
                            .context(activity)
                            .build()
            )
            activity.runOnUiThread {
                activity.addContentView(
                        EMAdView(activity).apply {
                            visibility = View.GONE
                            emAdView = this
                            loadAd()
                            if (emAdsModuleInput.isAutoPlay) {
                                showAd()
                            }
                            setContentController(
                                    object : EmClientContentController {
                                        override fun pauseContent() {
                                            Log.d("EngageAdsPlugin", "Pause Content")
                                            // notify unity to stop the content
                                            UnityPlayer.currentActivity?.runOnUiThread {
                                                UnityPlayer.UnitySendMessage(
                                                        "EngageAdsUnityBridge",
                                                        "OnPauseContent",
                                                        ""
                                                )
                                            }
                                        }

                                        override fun resumeContent() {
                                            Log.d("EngageAdsPlugin", "Resume Content")
                                            // notify unity to resume the content
                                            UnityPlayer.currentActivity?.runOnUiThread {
                                                UnityPlayer.UnitySendMessage(
                                                        "EngageAdsUnityBridge",
                                                        "OnResumeContent",
                                                        ""
                                                )
                                            }
                                        }
                                    }
                            )
                            setAdEventListener(
                                    object : EMVideoPlayerListener {
                                        override fun onAdEnded() {
                                            Log.d("EngageAdsPlugin", "Ad Ended")
                                            emAdView.visibility = View.GONE
                                            UnityPlayer.currentActivity?.runOnUiThread {
                                                UnityPlayer.UnitySendMessage(
                                                        "EngageAdsUnityBridge",
                                                        "OnAdEnded",
                                                        ""
                                                )
                                            }
                                        }

                                        override fun onAdLoading() {
                                            Log.d("EngageAdsPlugin", "Ad Loading")
                                        }

                                        override fun onAdPaused() {
                                            Log.d("EngageAdsPlugin", "Ad Paused")
                                        }

                                        override fun onAdResumed() {
                                            Log.d("EngageAdsPlugin", "Ad Resumed")
                                        }

                                        override fun onAdStarted() {
                                            Log.d("EngageAdsPlugin", "Ad Started")
                                            emAdView.visibility = View.VISIBLE
                                            UnityPlayer.currentActivity?.runOnUiThread {
                                                UnityPlayer.UnitySendMessage(
                                                        "EngageAdsUnityBridge",
                                                        "OnAdStarted",
                                                        ""
                                                )
                                            }
                                        }

                                        override fun onAdsLoaded() {
                                            Log.d("EngageAdsPlugin", "Ads Loaded")
                                            // find unity view and notify unity that ads are loaded
                                            UnityPlayer.currentActivity?.runOnUiThread {
                                                UnityPlayer.UnitySendMessage(
                                                        "EngageAdsPlugin",
                                                        "OnAdsLoaded",
                                                        ""
                                                )
                                            }
                                        }
                                    }
                            )
                        },
                        ViewGroup.LayoutParams(
                                ViewGroup.LayoutParams.MATCH_PARENT,
                                ViewGroup.LayoutParams.MATCH_PARENT
                        )
                )
                Log.d("EngageAdsPlugin", "Engage Ads SDK Initialized")
            }
        } catch (e: Exception) {
            Log.e("EngageAdsPlugin", "Failed to initialize Engage Ads SDK: " + e.message)
        }
    }

    fun loadAd() {
        UnityPlayer.currentActivity.runOnUiThread() { emAdView.loadAd() }
    }

    fun showAd() {
        UnityPlayer.currentActivity.runOnUiThread() { emAdView.showAd() }
    }
}
