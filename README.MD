# Preprocessor of the Old Church Slavonic texts

This tool, built with Windows Forms, enables a researcher to train HMM taggers and lemmatizers, as well as scrape texts from TITUS and CCMH, tokenize, preprocess, and tag them.

// Add a picture here

## Requirements

* Visual Studio 2019
* Python (for taggers and lemmatizers training), optional
* HMM PoS tagger (https://github.com/The-One-Who-Speaks-and-Depicts/hmm-pos-tagger), optional
* Lemmatiser (https://github.com/The-One-Who-Speaks-and-Depicts/OCS-corpus-lemmatiser), optional)

## How to install

* Clone the repository on the computer
* Run project (.csproj file) from Visual Studio 2019

## How to use

### Training

// Add a picture here

If one wants to train their own models for PoS tagging / lemmatisation, one should use "Обучить модель на CoNLL-U данных" option, where the upper button is for PoS tagging (may be implemented via HMM PoS Tagger), and the lower is for lemmatisation (may be implemented via Lemmatiser higher).

### Using

// Add a picture here

If one wants to scrape CCMH/TITUS data, one should use "Создать новый документ в корпусе" window. 

* Firstly, one should press "Загрузить текст со страницы". while the corresponding text field is filled with link from CCMH/TITUS [deprecated for CCMH, choose any link], to load text from web.
* Secondly, one should press "Выделить текст источника и метаданные" button [for CCMH, now: copy text from frame in web and insert into "Оригинальный текст" text field] to get the raw text.
* Thirdly, one should check, whether U presented izhitsa, or uk, in an original text, and pick option "ижица" or "ук" respectively. If U is not presented in the original text, one may pick any option. After that, one should press "ASCII => Unicode" button.
* Fourthly, to join word forms, that are located on different strings, in the text, additionally tokenize the text, and split it by punctuation marks, one should use "Соединить текст", "Токенизировать текст" and "Разбить текст по знакам пунктуации" buttons respectively.
* Finally, one should choose the period of text creation (for reference one may use Kamphuis J. (2020) Verbal Aspect in Old Church Slavonic: A Corpus-Based Approach, chapter 1, pp. 4 - 6), and then click on "Занести текст в базу" button.
* The user will be asked the folder, where their DB is located, then, whether to make PoS tagging and lemmatisation, then, if the answer to the latter is postive, the folder where HMM PoS Tagger, and Lemmatiser, are located. 