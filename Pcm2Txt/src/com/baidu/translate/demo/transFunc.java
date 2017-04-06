package com.baidu.translate.demo;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.util.List;

import com.google.gson.Gson;

public class transFunc {
	// 在平台申请的APP_ID 详见
	// http://api.fanyi.baidu.com/api/trans/product/desktop?req=developer
	private static final String APP_ID = "20170223000039745";
	private static final String SECURITY_KEY = "ZdmU93BM_0IjZRJ7jURU";

	///////////////////////////////////////////////////////////////
	// function used for calling the transApi with three parameters source file
	/////////////////////////////////////////////////////////////// path, source
	/////////////////////////////////////////////////////////////// language,
	/////////////////////////////////////////////////////////////// target
	/////////////////////////////////////////////////////////////// language
	public static void trans(String srcPath, String dstPath, String srcLan, String targetLan)
			throws FileNotFoundException, IOException {
		TransApi api = new TransApi(APP_ID, SECURITY_KEY);

		String query = null;
		// System.out.println(api.getTransResult(query, "auto", "en"));
		////////////
		FileReader fr = new FileReader(srcPath);
		BufferedReader br = new BufferedReader(fr);
		FileWriter fw = new FileWriter(dstPath);
		BufferedWriter bw = new BufferedWriter(fw);
		String line = "";
		String[] arrs = null;
		int i = 1;
		while ((line = br.readLine()) != null) {

			// System.out.println(i+": ");
			// System.out.println(line);
			arrs = line.split("  ");
			// System.out.println(arrs[0]);
			bw.write(String.valueOf(i));
			bw.newLine();
			String[] temp = arrs[0].split("-");
			bw.write(temp[0] + "-->" + temp[1]);
			bw.newLine();
			query = arrs[1];
			Gson gson = new Gson();
			String response = api.getTransResult(query, "auto", targetLan);
			// System.out.println(response);
			BaiduTrans bt = gson.fromJson(response, BaiduTrans.class);
			for (TransResult tr : bt.getTrans_result()) {
				// System.out.println(tr.getDst());
				bw.write(tr.getDst());
			}
			bw.newLine();
			bw.write(arrs[1]);
			bw.newLine();
			bw.newLine();
			// System.out.println(api.getTransResult(query, "auto", "en"));
			// System.out.println(arrs[1]);
			i++;
		}
		br.close();
		fr.close();
		bw.close();
		fw.close();
		////////////
	}

	////////////////////////////////////////////////////////////
	// classes used for analyzing the returned json
	class TransResult {
		public String getSrc() {
			return src;
		}

		public void setSrc(String src) {
			this.src = src;
		}

		public String getDst() {
			return dst;
		}

		public void setDst(String dst) {
			this.dst = dst;
		}

		private String src;
		private String dst;
	}

	class BaiduTrans {
		private String from;
		private String to;
		private List<TransResult> trans_result;

		public String getFrom() {
			return from;
		}

		public void setFrom(String from) {
			this.from = from;
		}

		public String getTo() {
			return to;
		}

		public void setTo(String to) {
			this.to = to;
		}

		public List<TransResult> getTrans_result() {
			return trans_result;
		}

		public void setTrans_result(List<TransResult> trans_result) {
			this.trans_result = trans_result;
		}
	}
	////////////////////////////////////////////////////////////

}
