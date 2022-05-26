#from distutils.debug import DEBUG
#from glob import escape
#from unicodedata import name
from flask import Flask, request, jsonify

# mysql 라이브러리 임포트: pip install pymysql
import pymysql

# 환경변수 라이브러리 임포트 다운: pip install python-dotenv
from dotenv import load_dotenv
import os

load_dotenv()

app = Flask(__name__)

@app.route('/')
def index():
    return jsonify(200)


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


@app.route('/initData/<string:name>')
def initData(name):
    conn = pymysql.connect(host='localhost', user=os.environ.get('DBuser'), password=os.environ.get('DBpass'), db='study_db', charset='utf8')
    try:
        cur = conn.cursor()

        sql = 'INSERT INTO users (name) VALUES(%s);'
        vals = (name)
        cur.execute(sql, vals)
        result = cur.fetchall()
        conn.commit()
    finally:
        cur.close()
        conn.close()

    return f'{result}'

@app.route('/getData', methods=['GET', 'POST'])
def getName():
    if request.method =="POST":
        params = request.get_json()

        conn = pymysql.connect(host='localhost', user=os.environ.get('DBuser'), password=os.environ.get('DBpass'), db='study_db', charset='utf8')
        cur = conn.cursor()

        sql = 'SELECT * FROM users WHERE _id = %s;'
        vals = params['id']
        cur.execute(sql, vals)

        rows = cur.fetchall()
        print(rows)

        cur.close()
        conn.close()
        return jsonify(rows)
    
    return 1

@app.route("/register", methods=['POST'])
def register_proc():
    params = request.get_json()

    u_id = params['uId']
    u_pw = params['uPw']
    u_name = params['uName']

    conn = pymysql.connect(host='localhost', user=os.environ.get('DBuser'), password=os.environ.get('DBpass'), db='study_db', charset='utf8')
    cur = conn.cursor()

    sql = 'SELECT * FROM users WHERE uId = %s;'
    cur.execute(sql, u_id)

    rows = cur.fetchone()
    print(rows)

    cur.close()
    conn.close()

    return sql

@app.route("/login", methods=['POST'])
def login_proc():
	input_data = request.form()
	user_id = input_data['id']
	user_pw = input_data['pw']

	# 아이디, 비밀번호가 일치하는 경우
	if (user_id == admin_id and
			user_pw == admin_pw):
		payload = {
			'id': user_id,
			'exp': datetime.utcnow() + timedelta(seconds=60)  # 로그인 24시간 유지
		}
		token = jwt.encode(payload, SECRET_KEY, algorithm='HS256')

		return jsonify({'result': 'success', 'token': token})


	# 아이디, 비밀번호가 일치하지 않는 경우
	else:
		return jsonify({'result': 'fail'})

if __name__ == '__main__':
    app.run(host="0.0.0.0", port=5000)