package com.example.caffwebshop.model

import com.google.gson.annotations.SerializedName

data class CaffItemPublic (
    @SerializedName("id")
    val id: Int,
    @SerializedName("name")
    val name: String,
    @SerializedName("description")
    val description: String,
    @SerializedName("ownerId")
    val ownerId: Int,
    @SerializedName("uploadedAt")
    val uploadedAt: Int,
    @SerializedName("owner")
    val owner: UserPublic
)