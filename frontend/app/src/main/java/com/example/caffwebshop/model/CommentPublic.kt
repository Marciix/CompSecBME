package com.example.caffwebshop.model

import com.google.gson.annotations.SerializedName

data class CommentPublic (
    @SerializedName("id")
    val id: Int,
    @SerializedName("content")
    val content: String,
    @SerializedName("createdAt")
    val createdAt: String,
    @SerializedName("authorId")
    val authorId: Int,
    @SerializedName("caffItemId")
    val caffItemId: Int,
    @SerializedName("author")
    val author: UserPublic
)