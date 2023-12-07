#!/usr/bin/env python3

from flask import Flask, request

app = Flask(__name__)
counter = 0
large_message = None
small_message = None

def increment_counter():
    global counter
    counter += 1
    print('Counter: %d' % counter)

@app.route('/post.html', methods=['POST'])
def post_test():
    print(request.form['message'])
    return request.form['message']

@app.route('/small.html', methods=['GET'])
def small_message_test():
    increment_counter()
    global small_message
    return small_message

@app.route('/very_small.html', methods=['GET'])
def very_small_message_test():
    increment_counter()
    global very_small_message
    return very_small_message

@app.route('/large.html', methods=['GET'])
def large_message_test():
    increment_counter()
    global large_message
    return large_message

if __name__ == '__main__':
    very_small_message = 'Very small message.'
    small_message = 'Small message from local computer.'
    counter = 0
    large_message = ''
    while (len(large_message) < 20000):
        large_message += '%d,' % counter
        counter += 1
    counter = 0
    app.run(host='0.0.0.0', port=8080)
