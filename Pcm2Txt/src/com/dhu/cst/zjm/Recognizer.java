package com.dhu.cst.zjm;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import com.iflytek.cloud.speech.LexiconListener;
import com.iflytek.cloud.speech.RecognizerListener;
import com.iflytek.cloud.speech.RecognizerResult;
import com.iflytek.cloud.speech.SpeechConstant;
import com.iflytek.cloud.speech.SpeechError;
import com.iflytek.cloud.speech.SpeechRecognizer;
import com.iflytek.cloud.speech.SpeechUtility;
import com.iflytek.cloud.speech.UserWords;

public class Recognizer {

	private static final String APPID = "58b3cacc";
	private StringBuffer mStringBuffer;
	boolean state = false;
	private ArrayList<byte[]> mArrayList;
	private int nowNum;
	private List<String> resultList = new ArrayList<String>();
	private int fileTime;
	private String endTime;
	private String lastTime = "00:00:00.000";

	public Recognizer() {
		SpeechUtility.createUtility("appid=" + APPID);
		mStringBuffer = new StringBuffer();
	}

	public List<String> getResList() {
		return resultList;
	}

	// *************************************词表上传*************************************

	/**
	 * 词表上传
	 */
	public void uploadUserWords(String USER_WORDS) {
		SpeechRecognizer recognizer = SpeechRecognizer.getRecognizer();
		if (recognizer == null) {
			recognizer = SpeechRecognizer.createRecognizer();

			if (null == recognizer) {
				System.out.println("获取识别实例实败！");
				return;
			}
		}

		UserWords userwords = new UserWords(USER_WORDS);
		recognizer.setParameter(SpeechConstant.DATA_TYPE, "userword");
		recognizer.updateLexicon("userwords", userwords.toString(), lexiconListener);
	}

	/**
	 * 词表上传监听器
	 */
	private LexiconListener lexiconListener = new LexiconListener() {
		@Override
		public void onLexiconUpdated(String lexiconId, SpeechError error) {
			if (error == null)
				System.out.println("*************上传成功*************");
			else
				System.out.println("*************" + error.getErrorCode() + "*************");
		}

	};

	// *************************************音频流听写*************************************

	/**
	 * 听写
	 */
	public int Recognize(int date, String tarLan) {
		if (SpeechRecognizer.getRecognizer() == null)
			SpeechRecognizer.createRecognizer();
		return RecognizePcmfileByte(date, tarLan);
	}

	/**
	 * 听写监听器
	 */
	RecognizerListener recListener = new RecognizerListener() {

		@Override
		public void onBeginOfSpeech() {
			DebugLog.Log("onBeginOfSpeech enter");
			System.out.println("*************开始录音*************");
		}

		@Override
		public void onEndOfSpeech() {
			System.out.println("onEndOfSpeech enter");
		}

		@Override
		public void onVolumeChanged(int volume) {
			System.out.println("onVolumeChanged enter");

		}

		@Override
		public void onResult(RecognizerResult result, boolean islast) {
			mStringBuffer.append(result.getResultString());
			if (islast) {
				System.out.println("单句识别结果为:" + mStringBuffer.toString());
				if (!mStringBuffer.toString().equals("")) {
					double resTime = (double) nowNum / mArrayList.size() * fileTime;
					String time = formatTime(resTime);
					resultList.add(lastTime + "-" + time + "  " + mStringBuffer.toString());
					lastTime = time;
				}
				mStringBuffer.delete(0, mStringBuffer.length());
				state = true;
			}
		}

		@Override
		public void onError(SpeechError error) {

			System.out.println("*************" + error.getErrorCode() + "*************");
		}

		@Override
		public void onEvent(int eventType, int arg1, int agr2, String msg) {
			DebugLog.Log("onEvent enter");
		}

	};

	/**
	 * 自动化测试注意要点 如果直接从音频文件识别，需要模拟真实的音速，防止音频队列的堵塞
	 */
	private int RecognizePcmfileByte(int date, String tarLan) {

		// 2、音频流听写
		SpeechRecognizer recognizer = SpeechRecognizer.getRecognizer();
		recognizer.setParameter(SpeechConstant.DOMAIN, "iat");
		if (tarLan.equals("en")) {
			recognizer.setParameter(SpeechConstant.LANGUAGE, "en_us");
		} else {
			recognizer.setParameter(SpeechConstant.LANGUAGE, "zh_cn");
			recognizer.setParameter(SpeechConstant.ACCENT, "mandarin ");
		}

		recognizer.setParameter(SpeechConstant.AUDIO_SOURCE, "-1");
		recognizer.setParameter(SpeechConstant.RESULT_TYPE, "plain");
		recognizer.startListening(recListener);

		state = false;
		for (int i = date; i < mArrayList.size(); i++) {
			recognizer.writeAudio(mArrayList.get(i), 0, mArrayList.get(i).length);
			System.out.println("总进度：" + mArrayList.size() + " 当前进度：" + i);
			nowNum = i;
			if (state) {
				System.out.println("单句识别结束");
				recognizer.stopListening();
				return nowNum + 1;
			}
			try {
				Thread.sleep(150);
			} catch (InterruptedException e) {
				e.printStackTrace();
			}
		}
		System.out.println("单句识别结束");
		recognizer.stopListening();
		return nowNum + 1;

	}

	public int readFile(String pcmName, int time, int len) {
		this.fileTime = time;
		endTime = formatEndTime(time);
		// 1、读取音频文件
		FileInputStream fis = null;
		byte[] voiceBuffer = null;
		try {
			fis = new FileInputStream(new File(pcmName));
			voiceBuffer = new byte[fis.available()];
			fis.read(voiceBuffer);
		} catch (Exception e) {
			e.printStackTrace();
		} finally {
			try {
				if (null != fis) {
					fis.close();
					fis = null;
				}
			} catch (IOException e) {
				e.printStackTrace();
			}
		}
		if (0 == voiceBuffer.length) {
			mStringBuffer.append("no audio avaible!");
			return 0;
		} else {
			// 每次写入msc数据4.8K,相当150ms录音数据
			mArrayList = splitBuffer(voiceBuffer, voiceBuffer.length, len);
			return mArrayList.size();
		}
	}

	/**
	 * 将字节缓冲区按照固定大小进行分割成数组
	 * 
	 * @param buffer
	 *            缓冲区
	 * @param length
	 *            缓冲区大小
	 * @param spsize
	 *            切割块大小
	 * @return
	 */
	private ArrayList<byte[]> splitBuffer(byte[] buffer, int length, int spsize) {
		ArrayList<byte[]> array = new ArrayList<byte[]>();
		if (spsize <= 0 || length <= 0 || buffer == null || buffer.length < length)
			return array;
		int size = 0;
		while (size < length) {
			int left = length - size;
			if (spsize < left) {
				byte[] sdata = new byte[spsize];
				System.arraycopy(buffer, size, sdata, 0, spsize);
				array.add(sdata);
				size += spsize;
			} else {
				byte[] sdata = new byte[left];
				System.arraycopy(buffer, size, sdata, 0, left);
				array.add(sdata);
				size += left;
			}
		}
		return array;
	}

	private String formatTime(double time) {
		long mss = (long) (time * 1000);
		long hours = (mss % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60);
		long minutes = (mss % (1000 * 60 * 60)) / (1000 * 60);
		long seconds = (mss % (1000 * 60)) / 1000;
		long milliseconds = mss % 1000;
		String res = time2String(hours) + ":" + time2String(minutes) + ":" + time2String(seconds) + "."
				+ time2String(milliseconds);
		System.out.println(res);
		return res;
	}

	private String formatEndTime(int time) {
		long mss = (long) (time * 1000);
		long hours = (mss % (60 * 60 * 24)) / (60 * 60);
		long minutes = (mss % (60 * 60)) / 60;
		long seconds = mss % 60;
		String res = time2String(hours) + ":" + time2String(minutes) + ":" + time2String(seconds) + ".000";
		System.out.println(res);
		return res;
	}

	private String time2String(long time) {
		if (time == 0) {
			return "00";
		} else {
			return time + "";
		}
	}

}
