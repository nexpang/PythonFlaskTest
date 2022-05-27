#from distutils.debug import DEBUG
#from glob import escape
#from unicodedata import name
#from datetime import datetime, timedelta
from flask import Flask, request, jsonify
import bcrypt
import jwt

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

"""
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
""" 
@app.route("/register", methods=['POST'])
def register_proc():
    params = request.get_json()

    u_id = params['uId']
    u_pw = params['uPw']
    u_name = params['uName']

    conn = pymysql.connect(host='localhost', user=os.environ.get('DBuser'), password=os.environ.get('DBpass'), db='study_db', charset='utf8')
    cur = conn.cursor()

    sql = 'SELECT EXISTS(SELECT * FROM users WHERE u_id = %s);'
    result = cur.execute(sql, u_id)

    result = cur.fetchone()[0]

    if result > 0:
        cur.close()
        conn.close()
        return jsonify({'result': False, 'str':'이미 같은 아이디가 존재합니다.'})

    sql = 'INSERT INTO users(u_id, u_pass, u_name) VALUES (%s, %s, %s);'
    encrypted_password = bcrypt.hashpw(u_pw.encode("utf-8"), bcrypt.gensalt())
    cur.execute(sql, (u_id,  encrypted_password.decode("utf-8"), u_name))
    
    conn.commit()

    cur.close()
    conn.close()
    
    return jsonify({'result': True, 'str':'회원가입 성공'})

@app.route("/login", methods=['POST']) 
def login_proc():
    params = request.get_json()
    u_id = params['uId']
    u_pw = params['uPw']
    
    conn = pymysql.connect(host='localhost', user=os.environ.get('DBuser'), password=os.environ.get('DBpass'), db='study_db', charset='utf8')
    cur = conn.cursor()

    sql = 'SELECT id, u_pass FROM users WHERE u_id = %s;'
    result = cur.execute(sql, u_id)
    
    cur.close()
    conn.close()
    
    if(result <= 0):
        return jsonify({'result':False, 'str':'해당하는 아이디가 존재하지 않습니다.'})
    

    rows = cur.fetchall()
    row = rows[0]
    id = row[0]
    admin_pw = row[1]

    # 아이디, 비밀번호가 일치하는 경우
    if (bcrypt.checkpw(u_pw.encode('utf-8'), admin_pw.encode('utf-8'))):
        payload = {
            'id' : id,
            #'exp' : datetime.utcnow + timedelta(seconds=60)
        }
        token = jwt.encode(payload, os.environ.get('SECRET_KEY'), algorithm='HS256')
        return jsonify({'result': True,'str':'로그인 성공' ,'token': token})
	# 아이디, 비밀번호가 일치하지 않는 경우
    else:
        return jsonify({'result': False, 'str':'비밀번호가 일치하지 않습니다.'})
    


if __name__ == '__main__':
    app.run(host="0.0.0.0", port=5000)