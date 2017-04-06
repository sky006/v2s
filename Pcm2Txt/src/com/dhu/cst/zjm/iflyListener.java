package com.dhu.cst.zjm;

import java.io.BufferedWriter;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileWriter;
import java.io.IOException;
import java.util.List;

import com.baidu.translate.demo.transFunc;

public class iflyListener {

	private Recognizer recognizer;
	private String pcmName;
	private int Time;
	private int len = 4800;
	private String txtName = "result.txt";
	private String USER_WORDS = "{\"userword\":[{\"name\":\"Moonlight\",\"words\":[\"gracious\",\"pageant\",\"restored\",\"classy\",\"dude\",\"adapted\"]},{\"name\":\"my\",\"words\":[\"back-up\",\"confused\",\"firm\",\"cool\"]}]}";

	public static void main(String args[]) {
		transfer(args[0], Integer.parseInt(args[1]), args[2], args[3], args[4], args[5]);
	}

	public static void transfer(String fileName, int len, String resultFile, String dstFile, String srcLan,
			String tarLan) {
		// 在应用发布版本中，请勿显示日志，详情见此函数说明。
		iflyListener m = new iflyListener();
		m.setName(fileName);
		m.setTime(len);
		m.setTextName(resultFile);
		m.setLen(4800);
		m.listen(srcLan);

		// translate
		try {
			transFunc.trans(resultFile, dstFile, "auto", tarLan);
		} catch (FileNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	public iflyListener() {
		recognizer = new Recognizer();
	}

	public void setName(String name) {
		this.pcmName = name;
	}

	public void setTime(int time) {
		this.Time = time;
	}

	public void setLen(int len) {
		this.len = len;
	}

	public void setTextName(String txt) {
		this.txtName = txt;
	}

	public void uploadWord(String USER_WORDS) {
		this.USER_WORDS = USER_WORDS;
		recognizer.uploadUserWords(USER_WORDS);
	}

	public void listen(String tarLan) {
		int i = 0;
		List<String> reString;
		String result;
		int k = recognizer.readFile(pcmName, Time, len);
		while (true) {
			i = recognizer.Recognize(i, tarLan);
			try {
				Thread.sleep(1500);
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
			if (k == i) {
				reString = recognizer.getResList();
				break;
			}
		}
		if (reString != null) {
			for (int j = 0; j < reString.size(); j++) {
				DebugLog.Log("第" + (j + 1) + "个结果" + reString.get(j));
			}
			try {
				File file = new File(txtName);
				if (!file.exists()) {
					file.createNewFile();
				}
				FileWriter fileWritter = new FileWriter(file.getAbsolutePath(), true);
				BufferedWriter bufferWritter = new BufferedWriter(fileWritter);
				for (int j = 0; j < reString.size(); j++) {
					bufferWritter.write(reString.get(j) + "\r\n");
				}
				bufferWritter.close();
				System.out.println("Done");
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}

	}
}
