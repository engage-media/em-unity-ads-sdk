package com.engage.unityvideoplugin

import android.app.Activity
import android.util.Log
import android.view.ViewGroup
import androidx.core.view.isVisible
import com.engage.engageadssdk.EMAdView
import com.engage.engageadssdk.EMVideoPlayerListener
import com.engage.engageadssdk.module.EMAdsModule
import com.engage.engageadssdk.module.EMAdsModuleInput
import com.engage.engageadssdk.module.EMAdsModuleInputBuilder
import com.engage.engageadssdk.ui.EmClientContentController
import org.json.JSONObject
import com.unity3d.player.UnityPlayer

class EMAdsPlugin {

    private lateinit var emAdView: EMAdView
    private val emAdsModuleInput: EMAdsModuleInput by lazy {
        EMAdsModule.getInstance()
    }

    fun initialize(activity: Activity, adsConfigJsonString: String) {

        try {
            val config: JSONObject = JSONObject(adsConfigJsonString)

            // Initialize the Engage SDK using the parsed config
            EMAdsModule.init(EMAdsModuleInputBuilder().apply {
                isDebug(config.optBoolean("isTestMode", false))
                bundleId(config.optString("bundleId", ""))
                publisherId(config.optString("publisherId") ?: run {
                    error("publisherId is required")
                })
                channelId(config.optString("channelId") ?: run {
                    error("channelId is required")
                })
                isAutoPlay(config.optBoolean("isAutoPlay", false))
                baseUrl(config.optString("baseUrl", ""))
                isGdprApproved(false)
                context(activity)
            }.build())
            activity.addContentView(
                EMAdView(activity)
                    .apply {
                        isVisible = false
                        emAdView = this
                        loadAd()
                        if (emAdsModuleInput.isAutoPlay) {
                            showAd()
                        }
                        setContentController(object : EmClientContentController {
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
                        })
                        setAdEventListener(object : EMVideoPlayerListener {
                            override fun onAdEnded() {
                                Log.d("EngageAdsPlugin", "Ad Ended")
                                emAdView.isVisible = false
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
                                emAdView.isVisible = true
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
                        })
                    },
                ViewGroup.LayoutParams(
                    ViewGroup.LayoutParams.MATCH_PARENT,
                    ViewGroup.LayoutParams.MATCH_PARENT
                )
            )
            Log.d("EngageAdsPlugin", "Engage Ads SDK Initialized")
        } catch (e: Exception) {
            Log.e("EngageAdsPlugin", "Failed to initialize Engage Ads SDK: " + e.message)
        }
    }

    fun loadAd() {
        emAdView.loadAd()
    }

    fun showAd() {
        emAdView.showAd()
    }

}