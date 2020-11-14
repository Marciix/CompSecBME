package com.example.caffwebshop.fragment

import android.app.Activity
import android.app.Dialog
import android.graphics.Color
import android.os.Bundle
import android.support.v4.app.DialogFragment
import android.support.v7.app.AlertDialog
import android.view.LayoutInflater
import android.view.View
import android.widget.EditText
import com.example.caffwebshop.R

class SearchDialogFragment: DialogFragment() {

    private lateinit var listener: SearchListener
    private lateinit var a: Activity
    private lateinit var etSearch: EditText
    private lateinit var viewAct: View

    interface SearchListener{
        fun search(searchParam: String)
    }

    override fun onStart() {
        super.onStart()
        (dialog as AlertDialog).getButton(AlertDialog.BUTTON_POSITIVE).setTextColor(Color.BLACK)
        (dialog as AlertDialog).getButton(AlertDialog.BUTTON_NEGATIVE).setTextColor(Color.BLACK)

    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        val act=this.activity
        a=this.activity as Activity

        if(act is SearchListener){
            listener=act
        }
        else{
            throw RuntimeException("Activity must implement SearchListener")
        }

    }

    override fun onCreateDialog(savedInstanceState: Bundle?): Dialog {

        val builder = AlertDialog.Builder(activity!!)

        builder.setView(getContentView())
        builder.setTitle("Search tag")
        builder.setPositiveButton("Ok") { _, _ ->
            listener.search(etSearch.text.toString())
        }
        builder.setNegativeButton("Cancel", null)
        return builder.create()

    }

    private fun getContentView(): View {
        viewAct = LayoutInflater.from(context).inflate(R.layout.search_fragment, null)
        etSearch=viewAct.findViewById(R.id.et_search)
        return viewAct
    }
}