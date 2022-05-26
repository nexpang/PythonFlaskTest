from glob import escape
from unicodedata import name
from flask import Flask, request

app = Flask(__name__)

@app.route('/')
def hello_world():
    name = request.args.get("name", "World")
    return f'Hell, {escape(name)}!'


@app.route('/calculator/<string:operation>/<int:v1>/<int:v2>')
def calculator(operation, v1, v2):
    if operation == "add":
        ret = v1 + v2
    elif operation == "sub":
        ret = v1 - v2
    elif operation == "mul":
        ret = v1 * v2
    elif operation == "div":
        ret = v1 / v2
    return f'Operation {operation} with {v1} and {v2} is {ret}'

if __name__ == '__main__':
    app.run(host="0.0.0.0", port=5000)