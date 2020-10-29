import pandas as pd
import re
import numpy as np
import keras, tensorflow
from keras.models import Model
from keras.layers import Input, LSTM, Dense
import argparse
import os

def load_train_dataset(datafile):
    with open(datafile, 'r', encoding='utf-8') as inp:
        strings = inp.readlines()
    dataset = pd.DataFrame(columns=['WORD', 'POS', 'LEMMA'])
    counter = 0
    for s in strings:
        split_string = s.split(' ')
        dataset.loc[counter] = [split_string[0], split_string[1], split_string[2]]
        counter = counter + 1
    return dataset

def model_training(dataset, folder):
    input_texts = []
    target_texts = []
    input_characters = set()
    target_characters = set()
    for index, row in dataset.iterrows():
      input_text = row['WORD']
      target_text = row['LEMMA']
      target_text = '\t' + target_text + '\n'
      input_texts.append(input_text)
      target_texts.append(target_text)
      for char in input_text:
        if char not in input_characters:
          input_characters.add(char)
      for char in target_text:
        if char not in target_characters:
          target_characters.add(char)
    input_characters = sorted(list(input_characters))
    target_characters = sorted(list(target_characters))
    num_encoder_tokens = len(input_characters)
    num_decoder_tokens = len(target_characters)
    max_encoder_seq_length = max([len(txt) for txt in input_texts])
    max_decoder_seq_length = max([len(txt) for txt in target_texts])
    input_token_index = dict([(char, i) for i, char in enumerate(input_characters)])
    target_token_index = dict([(char, i) for i, char in enumerate(target_characters)])
    encoder_input_data = np.zeros((len(input_texts), max_encoder_seq_length, num_encoder_tokens), dtype='float32')
    decoder_input_data = np.zeros((len(input_texts), max_decoder_seq_length, num_decoder_tokens), dtype='float32')
    decoder_target_data = np.zeros((len(input_texts), max_decoder_seq_length, num_decoder_tokens), dtype='float32')
    for i, (input_text, target_text) in enumerate(zip(input_texts, target_texts)):
      for t, char in enumerate(input_text):
        encoder_input_data[i, t, input_token_index[char]] = 1.
      for t, char in enumerate(target_text):
        # decoder_target_data is ahead of decoder_input_data by one timestep
        decoder_input_data[i, t, target_token_index[char]] = 1.
        if t > 0:
          # decoder_target_data will be ahead by one timestep
          # and will not include the start character.
          decoder_target_data[i, t - 1, target_token_index[char]] = 1.
    batch_size = 64  # batch size for training
    epochs = 1  # number of epochs to train for
    latent_dim = 256  # latent dimensionality of the encoding space
    encoder_inputs = Input(shape=(None, num_encoder_tokens))
    encoder = LSTM(latent_dim, return_state=True)
    encoder_outputs, state_h, state_c = encoder(encoder_inputs)
    encoder_states = [state_h, state_c]
    decoder_inputs = Input(shape=(None, num_decoder_tokens))
    decoder_lstm = LSTM(latent_dim, return_sequences=True, return_state=True)
    decoder_outputs, _, _ = decoder_lstm(decoder_inputs,
                                         initial_state=encoder_states)
    decoder_dense = Dense(num_decoder_tokens, activation='softmax')
    decoder_outputs = decoder_dense(decoder_outputs)
    model = Model(inputs=[encoder_inputs, decoder_inputs], 
              outputs=decoder_outputs)
    model.compile(optimizer='rmsprop', loss='categorical_crossentropy')
    model.fit([encoder_input_data, decoder_input_data], decoder_target_data,
          batch_size=batch_size,
          epochs=epochs,
          validation_split=0.2)
    model.save(args.folder + '\\seq2seq.h5')

def main(args):
    if (int(args.predict) == 0):
        train_dataset = load_train_dataset(args.data)
        model_training(train_dataset, args.folder)
    else:
        with open(args.folder + '\\result.txt', 'w') as out:
                for p in predictions:
                    out.write(p + '\n')
            

if __name__ == '__main__':    
    parser = argparse.ArgumentParser()
    parser.add_argument('--data', default=(os.path.dirname(os.path.realpath(__file__)) + "\\file.txt"))
    parser.add_argument('--folder', default=os.path.dirname(os.path.realpath(__file__)))
    parser.add_argument('--predict', default='0')
    args = parser.parse_args()
    main(args)