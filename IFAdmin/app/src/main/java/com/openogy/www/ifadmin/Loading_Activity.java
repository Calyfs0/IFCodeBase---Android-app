package com.openogy.www.ifadmin;

import android.content.Intent;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

public class Loading_Activity extends AppCompatActivity {

    Button btnAllAds, btnAllUsers,btnSearchUser,btnSearchAd;
    EditText editTextUser, editTextAd;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_loading_);

        btnAllAds = findViewById(R.id.btnAllAds);
        btnAllUsers = findViewById(R.id.btnAllUsers);
        btnSearchUser = findViewById(R.id.btnSearchUser);
        btnSearchAd = findViewById(R.id.btnSearchAd);
        editTextAd = findViewById(R.id.editTextAd);
        editTextUser = findViewById(R.id.editTextUserName);

        btnAllAds.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Intent intent = new Intent(Loading_Activity.this,MainActivity.class);
                startActivity(intent);
            }
        });

        btnAllUsers.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Intent intent = new Intent(Loading_Activity.this,AllUsers.class);
                startActivity(intent);
            }
        });

        btnSearchAd.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                String adString = editTextAd.getText().toString().trim();
                Intent intent = new Intent(Loading_Activity.this,SearchAdActivity.class);
                intent.putExtra("adString", adString);
                startActivity(intent);
            }
        });

        btnSearchUser.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                String userString = editTextUser.getText().toString().trim();
                Intent intent = new Intent(Loading_Activity.this,SearchUsers.class);
                intent.putExtra("userString", userString);
                startActivity(intent);
            }
        });
    }
}
