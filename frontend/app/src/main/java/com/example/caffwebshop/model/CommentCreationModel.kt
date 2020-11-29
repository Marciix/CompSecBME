package com.example.caffwebshop.model

import com.google.gson.annotations.SerializedName

data class CommentCreationModel (
    @SerializedName("content")
    val content:String
)