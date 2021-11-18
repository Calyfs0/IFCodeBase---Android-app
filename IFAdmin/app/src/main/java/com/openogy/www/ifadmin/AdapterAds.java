package com.openogy.www.ifadmin;

import android.app.Activity;
import android.app.Dialog;
import android.content.Context;
import android.graphics.drawable.ColorDrawable;
import android.os.AsyncTask;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.Window;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.io.Serializable;
import java.net.HttpURLConnection;
import java.net.ProtocolException;
import java.net.URL;
import java.text.NumberFormat;
import java.util.ArrayList;
import java.util.Locale;

public class AdapterAds extends ArrayAdapter<Ads> implements Serializable {
    private Activity activity;
    private ArrayList<Ads> Ads;
    private static LayoutInflater inflater = null;
    private Context context;
    private Dialog dialogForRepostInAnotherCity;
    private Dialog dialogForDelete;
    private Dialog dialogUpdateCategory;
    private Dialog dialogUpdateAd;
    private String highestBid;
    private ViewHolder holder;

    public AdapterAds(Activity activity, int textViewResourceId, ArrayList<Ads> _allAds, Context context) {
        super(activity, textViewResourceId, _allAds);

        try {
            this.context = context;
            this.activity = activity;
            this.Ads = _allAds;
            dialogForRepostInAnotherCity = new Dialog(context);
            dialogForDelete = new Dialog(context);
            dialogUpdateCategory = new Dialog(context);
            dialogUpdateAd = new Dialog(context);
            highestBid = "";

            inflater = (LayoutInflater) activity.getSystemService(Context.LAYOUT_INFLATER_SERVICE);

        } catch (Exception e) {

        }
    }

    public void addListItemToAdapter(ArrayList<Ads> list) {
        //add list to current arraylist of data
        Ads.addAll(list);

        //Notify UI
        this.notifyDataSetChanged();
    }

    public void updateResults() {

        //Triggers the list update
        this.notifyDataSetChanged();
    }

    public int getCount() {
        return Ads.size();
    }

    public Ads getItem(Ads position) {
        return position;
    }

    public long getItemId(int position) {
        return position;
    }

    public static class ViewHolder {
        public TextView display_name;
        public TextView display_title;
        public TextView display_sellingPrice;
        public TextView display_highestBid;
        public TextView display_locality;
        public ImageView display_adImage;
        public ProgressBar ImageProgressbar;
        public int position;
        public Button btnDemand;
        public Button btnDelete;
        public Button btnChangeCategory;
        public Button btnUpdateAd;
        public TextView display_currentCity;
        public TextView display_category;
    }

    public View getView(final int position, View convertView, ViewGroup parent) {
        View vi = convertView;

        try {
            if (convertView == null) {

                vi = inflater.inflate(R.layout.layout_singlead, null);
                holder = new ViewHolder();

                holder.display_name = vi.findViewById(R.id.tvVendorName);
                holder.display_locality = vi.findViewById(R.id.tvLocality);
                holder.display_title = vi.findViewById(R.id.tvAdTitle);
                holder.display_sellingPrice = vi.findViewById(R.id.tvSellingPrice);
                holder.display_highestBid = vi.findViewById(R.id.tvHighestBid);
                holder.display_adImage = vi.findViewById(R.id.IvAdImage);
                holder.ImageProgressbar = vi.findViewById(R.id.ImageProgressbar);
                holder.btnDemand = vi.findViewById(R.id.btnDemand);
                holder.btnDelete = vi.findViewById(R.id.btnDelete);
                holder.btnChangeCategory = vi.findViewById(R.id.btnUpdateCategory);
                holder.display_currentCity = vi.findViewById(R.id.CurrentCity);
                holder.display_category = vi.findViewById(R.id.CurrentCategory);
                holder.btnUpdateAd = vi.findViewById(R.id.btnUpdateAd);

                vi.setTag(holder);
            } else {
                holder = (ViewHolder) vi.getTag();
            }
            holder.display_category.setText(Ads.get(position).AdCategory);
            holder.display_currentCity.setText("Current City : " + Ads.get(position).AdCity);
            holder.display_name.setText(Ads.get(position).VendorName);
            holder.display_locality.setText(Ads.get(position).AdLocality);
            holder.display_title.setText(Ads.get(position).AdTitle);

            if((Ads.get(position).AdSellingPrice).equals("0"))
                holder.display_sellingPrice.setText("Yet to be discovered");
            else
                holder.display_sellingPrice.setText(getFormatedAmount(Integer.parseInt(Ads.get(position).AdSellingPrice)));
            holder.display_highestBid.setText((Ads.get(position).AdHighestBid).equals("0") ? "No bid" : getFormatedAmount(Integer.parseInt(Ads.get(position).AdHighestBid)));
            //byte[] bytes = android.util.Base64.decode((Ads.get(position).AdImageOne), 0);
            holder.ImageProgressbar.setVisibility(View.GONE);
            holder.display_adImage.setImageBitmap(Ads.get(position).AdImageBitmap);
            holder.position = position;

            holder.btnDelete.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    dialogForDelete.setContentView(R.layout.areyousure_delete_popup);
                    Button btnYes = dialogForDelete.findViewById(R.id.btnYes);
                    Button btnNo = dialogForDelete.findViewById(R.id.btnNo);
                    dialogForDelete.show();

                    btnYes.setOnClickListener(new View.OnClickListener() {
                        @Override
                        public void onClick(View view) {
                            new MarkAdAsDeletedPermanentlyAsyncTask(position).execute();
                        }
                    });

                    btnNo.setOnClickListener(new View.OnClickListener() {
                        @Override
                        public void onClick(View view) {
                            dialogForDelete.dismiss();
                        }
                    });
                }
            });

            holder.btnDemand.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {

                    dialogForRepostInAnotherCity.setContentView(R.layout.repostadpopup);
                    // dialog.setCancelable(false);
                    final EditText editTextCity = dialogForRepostInAnotherCity.findViewById(R.id.editTextCity);
                    final EditText editTextLocality = dialogForRepostInAnotherCity.findViewById(R.id.editTextLocality);
                    Button btnSubmit = dialogForRepostInAnotherCity.findViewById(R.id.btnSubmit);
                    final TextView txtResult = dialogForRepostInAnotherCity.findViewById(R.id.txtResult);
                    txtResult.setText("");
                    btnSubmit.setOnClickListener(new View.OnClickListener() {
                        @Override
                        public void onClick(View view) {
                                new UpdateAdCity(position, editTextCity.getText().toString().trim(),editTextLocality.getText().toString().trim()).execute();
                        }
                    });
                    Button btnCancel = dialogForRepostInAnotherCity.findViewById(R.id.btnCancel);
                    btnCancel.setOnClickListener(new View.OnClickListener() {
                        @Override
                        public void onClick(View view) {
                            dialogForRepostInAnotherCity.dismiss();
                        }
                    });

                    dialogForRepostInAnotherCity.show();
                }
            });

            holder.btnChangeCategory.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    dialogUpdateCategory.setContentView(R.layout.updatecategorypopup);
                    // dialog.setCancelable(false);
                    final EditText editTextCategory = dialogUpdateCategory.findViewById(R.id.editTextCategory);
                    Button btnSubmit = dialogUpdateCategory.findViewById(R.id.btnSubmit);
                    final TextView txtResult = dialogUpdateCategory.findViewById(R.id.txtResult);
                    txtResult.setText("");
                    btnSubmit.setOnClickListener(new View.OnClickListener() {
                        @Override
                        public void onClick(View view) {
                            new UpdateAdCategory(position, editTextCategory.getText().toString().trim()).execute();
                        }
                    });
                    Button btnCancel = dialogUpdateCategory.findViewById(R.id.btnCancel);
                    btnCancel.setOnClickListener(new View.OnClickListener() {
                        @Override
                        public void onClick(View view) {
                            dialogUpdateCategory.dismiss();
                        }
                    });

                    dialogUpdateCategory.show();
                }
            });

            holder.btnUpdateAd.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    dialogUpdateAd.setContentView(R.layout.updateadpopup);
                    // dialog.setCancelable(false);
                    final EditText editTextAdTitle = dialogUpdateAd.findViewById(R.id.editTextAdTitle);
                    editTextAdTitle.setText(Ads.get(position).AdTitle);
                    final EditText editTextAdDesc = dialogUpdateAd.findViewById(R.id.editTextAdDesc);
                    editTextAdDesc.setText(Ads.get(position).AdDescription);
                    Button btnSubmit = dialogUpdateAd.findViewById(R.id.btnSubmit);
                    final TextView txtResult = dialogUpdateAd.findViewById(R.id.txtResult);
                    txtResult.setText("");
                    btnSubmit.setOnClickListener(new View.OnClickListener() {
                        @Override
                        public void onClick(View view) {
                            new UpdateAdInfo(position, editTextAdTitle.getText().toString().trim(), editTextAdDesc.getText().toString().trim()).execute();
                        }
                    });
                    Button btnCancel = dialogUpdateAd.findViewById(R.id.btnCancel);
                    btnCancel.setOnClickListener(new View.OnClickListener() {
                        @Override
                        public void onClick(View view) {
                            dialogUpdateAd.dismiss();
                        }
                    });

                    dialogUpdateAd.show();
                }
            });

        } catch (Exception e) {
            e.printStackTrace();

        }
        return vi;
    }




    public class UpdateAdCity extends AsyncTask<Integer, Void, String> {

        private final int position;
        private String City;
        private String Locality;

        private Dialog dialog = new Dialog(getContext(), android.R.style.Theme_Black);

        private UpdateAdCity(int position, String City, String Locality) {
            this.position = position;
            this.City = City;
            this.Locality = Locality;
        }

        @Override
        protected void onPreExecute() {
            super.onPreExecute();
            View view = LayoutInflater.from(getContext()).inflate(R.layout.layout_progressbar, null);
            dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
            dialog.setCancelable(false);
            dialog.getWindow().setBackgroundDrawable(new ColorDrawable(android.graphics.Color.TRANSPARENT));
            dialog.setContentView(view);
            dialog.show();
        }

        @Override
        protected String doInBackground(Integer... params) {

            try {
                String urlAPI = "http://api.ifadvertisings.com/api/Ad/RepostInDifferentCity";
                URL url = new URL(urlAPI);
                //Create URL connection
                HttpURLConnection conn = null;
                conn = (HttpURLConnection) url.openConnection();
                conn.setRequestMethod("POST");
                conn.setRequestProperty("Content-Type", "application/json; charset=utf-8");

                //Create Json Object
                JSONObject jsonObject = new JSONObject();
                jsonObject.accumulate("AdId", Ads.get(position).AdId);
                jsonObject.accumulate("City", City);
                jsonObject.accumulate("Locality", Locality);
                //Post json content
                OutputStream os = conn.getOutputStream();
                BufferedWriter writer = new BufferedWriter(new OutputStreamWriter(os, "UTF-8"));
                writer.write(jsonObject.toString());
                writer.flush();
                writer.close();
                os.close();

                InputStreamReader inputStreamReader = new InputStreamReader(conn.getInputStream());
                BufferedReader bufferedReader = new BufferedReader(inputStreamReader);
                StringBuilder stringBuilder = new StringBuilder();
                String line;
                while ((line = bufferedReader.readLine()) != null)
                {
                    stringBuilder.append(line);
                }

                return conn.getResponseMessage();

            } catch (ProtocolException e) {
                return e.toString();
            } catch (IOException e) {
                return e.toString();
            } catch (JSONException e) {
                return e.toString();
            }
        }

        @Override
        protected void onPostExecute(String s) {

            if (s.equalsIgnoreCase("ok"))
            {
                
            }

            else
                Toast.makeText(activity, "Failed to update", Toast.LENGTH_SHORT).show();

            if(dialog.isShowing())
                dialog.dismiss();

            if(dialogForRepostInAnotherCity.isShowing())
                dialogForRepostInAnotherCity.dismiss();
        }
    }

    public class UpdateAdCategory extends AsyncTask<Integer, Void, String> {

        private final int position;
        private String Category;


        private Dialog dialog = new Dialog(getContext(), android.R.style.Theme_Black);

        private UpdateAdCategory(int position, String Category) {
            this.position = position;
            this.Category = Category;

        }

        @Override
        protected void onPreExecute() {
            super.onPreExecute();
            View view = LayoutInflater.from(getContext()).inflate(R.layout.layout_progressbar, null);
            dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
            dialog.setCancelable(false);
            dialog.getWindow().setBackgroundDrawable(new ColorDrawable(android.graphics.Color.TRANSPARENT));
            dialog.setContentView(view);
            dialog.show();
        }

        @Override
        protected String doInBackground(Integer... params) {

            try {
                String urlAPI = "http://api.ifadvertisings.com/api/Ad/UpdateAdCategory";
                URL url = new URL(urlAPI);
                //Create URL connection
                HttpURLConnection conn = null;
                conn = (HttpURLConnection) url.openConnection();
                conn.setRequestMethod("POST");
                conn.setRequestProperty("Content-Type", "application/json; charset=utf-8");

                //Create Json Object
                JSONObject jsonObject = new JSONObject();
                jsonObject.accumulate("AdId", Ads.get(position).AdId);
                jsonObject.accumulate("Category", Category);
                //Post json content
                OutputStream os = conn.getOutputStream();
                BufferedWriter writer = new BufferedWriter(new OutputStreamWriter(os, "UTF-8"));
                writer.write(jsonObject.toString());
                writer.flush();
                writer.close();
                os.close();

                InputStreamReader inputStreamReader = new InputStreamReader(conn.getInputStream());
                BufferedReader bufferedReader = new BufferedReader(inputStreamReader);
                StringBuilder stringBuilder = new StringBuilder();
                String line;
                while ((line = bufferedReader.readLine()) != null)
                {
                    stringBuilder.append(line);
                }

                return conn.getResponseMessage();

            } catch (ProtocolException e) {
                return e.toString();
            } catch (IOException e) {
                return e.toString();
            } catch (JSONException e) {
                return e.toString();
            }
        }

        @Override
        protected void onPostExecute(String s) {

            if (s.equalsIgnoreCase("ok"))
            {

            }

            else
                Toast.makeText(activity, "Failed to update", Toast.LENGTH_SHORT).show();

            if(dialog.isShowing())
                dialog.dismiss();

            if(dialogUpdateCategory.isShowing())
                dialogUpdateCategory.dismiss();
        }
    }

    public class UpdateAdInfo extends AsyncTask<Integer, Void, String> {

        private final int position;
        private String AdTitle;
        private String AdDesc;


        private Dialog dialog = new Dialog(getContext(), android.R.style.Theme_Black);

        private UpdateAdInfo(int position, String AdTitle, String AdDesc) {
            this.position = position;
            this.AdTitle = AdTitle;
            this.AdDesc = AdDesc;

        }

        @Override
        protected void onPreExecute() {
            super.onPreExecute();
            View view = LayoutInflater.from(getContext()).inflate(R.layout.layout_progressbar, null);
            dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
            dialog.setCancelable(false);
            dialog.getWindow().setBackgroundDrawable(new ColorDrawable(android.graphics.Color.TRANSPARENT));
            dialog.setContentView(view);
            dialog.show();
        }

        @Override
        protected String doInBackground(Integer... params) {

            try {
                String urlAPI = "http://api.ifadvertisings.com/api/Ad/UpdateAdInfo";
                URL url = new URL(urlAPI);
                //Create URL connection
                HttpURLConnection conn = null;
                conn = (HttpURLConnection) url.openConnection();
                conn.setRequestMethod("POST");
                conn.setRequestProperty("Content-Type", "application/json; charset=utf-8");

                //Create Json Object
                JSONObject jsonObject = new JSONObject();
                jsonObject.accumulate("AdId", Ads.get(position).AdId);
                jsonObject.accumulate("AdTitle", AdTitle);
                jsonObject.accumulate("AdDesc", AdDesc);
                //Post json content
                OutputStream os = conn.getOutputStream();
                BufferedWriter writer = new BufferedWriter(new OutputStreamWriter(os, "UTF-8"));
                writer.write(jsonObject.toString());
                writer.flush();
                writer.close();
                os.close();

                InputStreamReader inputStreamReader = new InputStreamReader(conn.getInputStream());
                BufferedReader bufferedReader = new BufferedReader(inputStreamReader);
                StringBuilder stringBuilder = new StringBuilder();
                String line;
                while ((line = bufferedReader.readLine()) != null)
                {
                    stringBuilder.append(line);
                }

                return conn.getResponseMessage();

            } catch (ProtocolException e) {
                return e.toString();
            } catch (IOException e) {
                return e.toString();
            } catch (JSONException e) {
                return e.toString();
            }
        }

        @Override
        protected void onPostExecute(String s) {

            if (s.equalsIgnoreCase("ok"))
            {

            }

            else
                Toast.makeText(activity, "Failed to update", Toast.LENGTH_SHORT).show();

            if(dialog.isShowing())
                dialog.dismiss();

            if(dialogUpdateAd.isShowing())
                dialogUpdateAd.dismiss();
        }
    }

    public class MarkAdAsDeletedPermanentlyAsyncTask extends AsyncTask<Integer, Void, String> {

        private final int position;

        private Dialog dialog = new Dialog(getContext(), android.R.style.Theme_Black);

        private MarkAdAsDeletedPermanentlyAsyncTask(int position) {
            this.position = position;
        }

        @Override
        protected void onPreExecute() {
            super.onPreExecute();
            View view = LayoutInflater.from(getContext()).inflate(R.layout.layout_progressbar, null);
            dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
            dialog.setCancelable(false);
            dialog.getWindow().setBackgroundDrawable(new ColorDrawable(android.graphics.Color.TRANSPARENT));
            dialog.setContentView(view);
            dialog.show();
        }

        @Override
        protected String doInBackground(Integer... params) {

            try {
                String urlAPI ="http://api.ifadvertisings.com/api/Ad/DeleteSelectedAdPermanently";
                URL url = new URL(urlAPI);
                //Create URL connection
                HttpURLConnection conn = null;
                conn = (HttpURLConnection) url.openConnection();
                conn.setRequestMethod("POST");
                conn.setRequestProperty("Content-Type", "application/json; charset=utf-8");

                //Create Json Object
                JSONObject jsonObject = new JSONObject();
                jsonObject.accumulate("AdId", Ads.get(position).AdId);

                //Post json content
                OutputStream os = conn.getOutputStream();
                BufferedWriter writer = new BufferedWriter(new OutputStreamWriter(os, "UTF-8"));
                writer.write(jsonObject.toString());
                writer.flush();
                writer.close();
                os.close();


                return conn.getResponseMessage();

            } catch (ProtocolException e) {
                return e.toString();
            } catch (IOException e) {
                return e.toString();
            } catch (JSONException e) {
                return e.toString();
            }
        }

        @Override
        protected void onPostExecute(String s) {

            if (s.equalsIgnoreCase("ok")) {
                remove(Ads.get(position));
                notifyDataSetChanged();
                Toast.makeText(activity, "Ad Removed Permanently", Toast.LENGTH_SHORT).show();
            } else
                Toast.makeText(context, "Faled to Remove ad", Toast.LENGTH_SHORT).show();

            if (dialog.isShowing()) {
                // Dismiss/hide the progress dialog
                dialog.dismiss();
            }

            if (dialogForDelete.isShowing()) {
                dialogForDelete.dismiss();
            }
        }
    }

    private String getFormatedAmount(int amount){
        return NumberFormat.getNumberInstance(Locale.getDefault()).format(amount);
    }


    }
