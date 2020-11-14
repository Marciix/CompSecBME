package com.example.caffwebshop.model

import com.google.gson.annotations.SerializedName

data class CaffItemPublic (
    @SerializedName("id")
    val id: Int,
    @SerializedName("title")
    val title: String,
    @SerializedName("tags")
    val tags: List<String>,
    @SerializedName("ownerId")
    val ownerId: Int,
    @SerializedName("uploadedAt")
    val uploadedAt: String,
    @SerializedName("owner")
    val owner: UserPublic
)